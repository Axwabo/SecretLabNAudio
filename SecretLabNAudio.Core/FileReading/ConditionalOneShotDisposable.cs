namespace SecretLabNAudio.Core.FileReading;

public sealed class ConditionalOneShotDisposable
{

    private readonly bool _closeOnDispose;
    private IDisposable? _disposable;

    public ConditionalOneShotDisposable(IDisposable disposable, bool closeOnDispose)
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
