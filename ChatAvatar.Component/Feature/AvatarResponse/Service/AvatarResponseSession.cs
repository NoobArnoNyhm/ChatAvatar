using ChatAvatar.Component.Feature.AvatarResponse.Modell;

namespace ChatAvatar.Component.Feature.AvatarResponse.Service;

public sealed class AvatarResponseSession : IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly TaskCompletionSource<AvatarResponseResult> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public AvatarResponseSession()
        : this(Guid.NewGuid())
    {
    }

    public AvatarResponseSession(Guid responseId)
    {
        if (responseId == Guid.Empty)
        {
            throw new ArgumentException("A response ID cannot be empty.", nameof(responseId));
        }

        ResponseId = responseId;
    }

    public Guid ResponseId { get; }

    public CancellationToken CancellationToken => _cancellationTokenSource.Token;

    public Task<AvatarResponseResult> Completion => _completion.Task;

    public bool Complete() => TryFinish(AvatarResponseStatus.Completed);

    public bool Cancel()
    {
        if (!TryFinish(AvatarResponseStatus.Cancelled))
        {
            return false;
        }

        _cancellationTokenSource.Cancel();
        return true;
    }

    public bool Fail(string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);
        return TryFinish(AvatarResponseStatus.Failed, errorMessage);
    }

    public void Dispose() => _cancellationTokenSource.Dispose();

    private bool TryFinish(AvatarResponseStatus status, string? errorMessage = null) =>
        _completion.TrySetResult(new AvatarResponseResult(ResponseId, status, errorMessage));
}