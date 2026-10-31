using System.Buffers.Binary;
using System.IO;
using ChatAvatar.Component.Featur.Avatar.Service;

namespace ChatAvatar.Tests.Featur.Avatar.Service;

[TestClass]
public sealed class WaveAudioLevelAnalyzerTests
{
    [TestMethod]
    public void AnalyzerReturnsRmsLevelForRequestedAudioWindow()
    {
        var analyzer = new WaveAudioLevelAnalyzer(CreatePcmWave(16384, -16384, 16384, -16384));

        var level = analyzer.GetRmsLevel(TimeSpan.Zero, TimeSpan.FromMilliseconds(2));

        Assert.AreEqual(0.5f, level, 0.001f);
    }

    [TestMethod]
    public void AnalyzerReturnsZeroForSilenceAndTimeAfterTheAudio()
    {
        var analyzer = new WaveAudioLevelAnalyzer(CreatePcmWave(0, 0, 0, 0));

        Assert.AreEqual(0, analyzer.GetRmsLevel(TimeSpan.Zero));
        Assert.AreEqual(0, analyzer.GetRmsLevel(analyzer.Duration));
    }

    [TestMethod]
    public void AnalyzerPreservesOriginalStreamPosition()
    {
        using var stream = CreatePcmWave(1000, 1000, 1000, 1000);
        stream.Position = 5;

        _ = new WaveAudioLevelAnalyzer(stream);

        Assert.AreEqual(5, stream.Position);
    }

    [TestMethod]
    public void AnalyzerRejectsNonWaveAndUnsupportedFormats()
    {
        Assert.ThrowsExactly<InvalidDataException>(() => new WaveAudioLevelAnalyzer(new MemoryStream([1, 2, 3])));
        Assert.ThrowsExactly<NotSupportedException>(() => new WaveAudioLevelAnalyzer(
            CreatePcmWave(new short[] { 1, 2, 3, 4 }, bitsPerSample: 12)));
    }

    private static MemoryStream CreatePcmWave(params short[] samples) => CreatePcmWave(samples, 16);

    private static MemoryStream CreatePcmWave(short[] samples, int bitsPerSample)
    {
        var bytesPerSample = bitsPerSample / 8;
        var dataLength = samples.Length * bytesPerSample;
        var bytes = new byte[44 + dataLength];
        "RIFF"u8.CopyTo(bytes);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(4, 4), bytes.Length - 8);
        "WAVEfmt "u8.CopyTo(bytes.AsSpan(8));
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(16, 4), 16);
        BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan(20, 2), 1);
        BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan(22, 2), 1);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(24, 4), 1000);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(28, 4), 1000 * bytesPerSample);
        BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan(32, 2), (short)bytesPerSample);
        BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan(34, 2), (short)bitsPerSample);
        "data"u8.CopyTo(bytes.AsSpan(36));
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(40, 4), dataLength);
        if (bitsPerSample == 16)
        {
            for (var index = 0; index < samples.Length; index++)
            {
                BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan(44 + index * 2, 2), samples[index]);
            }
        }

        return new MemoryStream(bytes);
    }
}