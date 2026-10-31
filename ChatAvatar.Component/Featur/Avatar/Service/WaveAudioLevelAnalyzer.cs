using System.Buffers.Binary;
using System.IO;

namespace ChatAvatar.Component.Featur.Avatar.Service;

public sealed class WaveAudioLevelAnalyzer : IAudioLevelAnalyzer
{
    private const ushort PcmFormat = 1;
    private const ushort IeeeFloatFormat = 3;
    private readonly byte[] _waveData;
    private readonly int _dataOffset;
    private readonly int _dataLength;
    private readonly int _sampleRate;
    private readonly int _channels;
    private readonly int _bitsPerSample;
    private readonly int _blockAlign;
    private readonly ushort _format;

    public WaveAudioLevelAnalyzer(Stream waveStream)
    {
        ArgumentNullException.ThrowIfNull(waveStream);
        if (!waveStream.CanRead || !waveStream.CanSeek)
        {
            throw new ArgumentException("WAV analysis requires a readable, seekable stream.", nameof(waveStream));
        }

        var originalPosition = waveStream.Position;
        try
        {
            waveStream.Position = 0;
            using var copy = new MemoryStream();
            waveStream.CopyTo(copy);
            _waveData = copy.ToArray();
        }
        finally
        {
            waveStream.Position = originalPosition;
        }

        if (_waveData.Length < 12
            || !HasFourCc(0, "RIFF")
            || !HasFourCc(8, "WAVE"))
        {
            throw new InvalidDataException("The audio stream is not a valid RIFF/WAVE file.");
        }

        ushort format = 0;
        int sampleRate = 0;
        int channels = 0;
        int bitsPerSample = 0;
        int blockAlign = 0;
        int dataOffset = -1;
        int dataLength = 0;

        var chunkOffset = 12;
        while (chunkOffset <= _waveData.Length - 8)
        {
            var chunkLengthValue = BinaryPrimitives.ReadUInt32LittleEndian(_waveData.AsSpan(chunkOffset + 4, 4));
            if (chunkLengthValue > int.MaxValue)
            {
                throw new InvalidDataException("A WAV chunk is too large to analyze.");
            }

            var chunkLength = (int)chunkLengthValue;
            var payloadOffset = chunkOffset + 8;
            if (chunkLength > _waveData.Length - payloadOffset)
            {
                throw new InvalidDataException("A WAV chunk extends beyond the end of the file.");
            }

            if (HasFourCc(chunkOffset, "fmt "))
            {
                if (chunkLength < 16)
                {
                    throw new InvalidDataException("The WAV format chunk is incomplete.");
                }

                format = BinaryPrimitives.ReadUInt16LittleEndian(_waveData.AsSpan(payloadOffset, 2));
                channels = BinaryPrimitives.ReadUInt16LittleEndian(_waveData.AsSpan(payloadOffset + 2, 2));
                sampleRate = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(_waveData.AsSpan(payloadOffset + 4, 4)));
                blockAlign = BinaryPrimitives.ReadUInt16LittleEndian(_waveData.AsSpan(payloadOffset + 12, 2));
                bitsPerSample = BinaryPrimitives.ReadUInt16LittleEndian(_waveData.AsSpan(payloadOffset + 14, 2));

                if (format == 0xFFFE && chunkLength >= 40)
                {
                    format = BinaryPrimitives.ReadUInt16LittleEndian(_waveData.AsSpan(payloadOffset + 24, 2));
                }
            }
            else if (HasFourCc(chunkOffset, "data"))
            {
                dataOffset = payloadOffset;
                dataLength = chunkLength;
            }

            chunkOffset = checked(payloadOffset + chunkLength + (chunkLength & 1));
        }

        if (dataOffset < 0 || channels <= 0 || sampleRate <= 0 || blockAlign <= 0)
        {
            throw new InvalidDataException("The WAV file is missing required format or sample data.");
        }

        if ((format != PcmFormat && format != IeeeFloatFormat)
            || (format == IeeeFloatFormat && bitsPerSample != 32)
            || (format == PcmFormat && bitsPerSample is not (8 or 16 or 24 or 32))
            || blockAlign != channels * (bitsPerSample / 8))
        {
            throw new NotSupportedException($"WAV format {format} with {bitsPerSample}-bit samples is not supported.");
        }

        _format = format;
        _dataOffset = dataOffset;
        _dataLength = dataLength;
        _sampleRate = sampleRate;
        _channels = channels;
        _bitsPerSample = bitsPerSample;
        _blockAlign = blockAlign;
    }

    public TimeSpan Duration => TimeSpan.FromSeconds(_dataLength / (double)_blockAlign / _sampleRate);

    public float GetRmsLevel(TimeSpan audioPosition, TimeSpan? window = null)
    {
        if (audioPosition < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(audioPosition));
        }

        var windowLength = window ?? TimeSpan.FromMilliseconds(20);
        if (windowLength <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(window));
        }

        if (audioPosition >= Duration)
        {
            return 0;
        }

        var firstFrame = (long)(audioPosition.TotalSeconds * _sampleRate);
        var frameCount = Math.Max(1, (long)Math.Ceiling(windowLength.TotalSeconds * _sampleRate));
        var totalFrames = _dataLength / _blockAlign;
        var framesToRead = Math.Min(frameCount, totalFrames - firstFrame);
        if (framesToRead <= 0)
        {
            return 0;
        }

        var totalSquared = 0d;
        var sampleCount = framesToRead * _channels;
        var sampleWidth = _bitsPerSample / 8;
        for (long frame = 0; frame < framesToRead; frame++)
        {
            var frameOffset = _dataOffset + checked((int)((firstFrame + frame) * _blockAlign));
            for (var channel = 0; channel < _channels; channel++)
            {
                var sampleOffset = frameOffset + channel * sampleWidth;
                var sample = ReadSample(sampleOffset);
                totalSquared += sample * sample;
            }
        }

        return (float)Math.Clamp(Math.Sqrt(totalSquared / sampleCount), 0d, 1d);
    }

    private double ReadSample(int offset)
    {
        if (_format == IeeeFloatFormat)
        {
            var bits = BinaryPrimitives.ReadInt32LittleEndian(_waveData.AsSpan(offset, 4));
            var value = BitConverter.Int32BitsToSingle(bits);
            return float.IsFinite(value) ? Math.Clamp(value, -1f, 1f) : 0;
        }

        return _bitsPerSample switch
        {
            8 => (_waveData[offset] - 128) / 128d,
            16 => BinaryPrimitives.ReadInt16LittleEndian(_waveData.AsSpan(offset, 2)) / 32768d,
            24 => ReadInt24(offset) / 8388608d,
            32 => BinaryPrimitives.ReadInt32LittleEndian(_waveData.AsSpan(offset, 4)) / 2147483648d,
            _ => 0
        };
    }

    private int ReadInt24(int offset)
    {
        var value = _waveData[offset] | (_waveData[offset + 1] << 8) | (_waveData[offset + 2] << 16);
        return (value & 0x800000) == 0 ? value : value | unchecked((int)0xFF000000);
    }

    private bool HasFourCc(int offset, string value) =>
        _waveData.AsSpan(offset, 4).SequenceEqual(System.Text.Encoding.ASCII.GetBytes(value));
}