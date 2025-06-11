namespace SecretLabNAudio.Core.FileReading;

public sealed class ConditionalOneTimeDisposable
{

    private readonly bool _closeOnDispose;
    private IDisposable? _disposable;

    public ConditionalOneTimeDisposable(IDisposable disposable, bool closeOnDispose)
    {
        _disposable = disposable;
        _closeOnDispose = closeOnDispose;
    }

    public void Dispose(bool disposing)
    {
        if (!disposing || _disposable == null)
            return;
        if (_closeOnDispose)
            _disposable.Dispose();
        _disposable = null;
    }

}
