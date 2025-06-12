namespace SecretLabNAudio.Core.FileReading;

/// <summary>
/// Conditionally disposes the given <see cref="IDisposable"/> when <see cref="Dispose"/> is called with disposing = true,
/// then sets the stored disposable to null.
/// </summary>
public sealed class ConditionalOneTimeDisposable
{

    private readonly bool _closeOnDispose;
    private IDisposable? _disposable;

    /// <summary>
    /// Creates a new <see cref="ConditionalOneTimeDisposable"/> that will dispose the given <see cref="IDisposable"/>
    /// </summary>
    /// <param name="disposable">The <see cref="IDisposable"/> to dispose.</param>
    /// <param name="closeOnDispose">Whether to call <see cref="IDisposable.Dispose"/> on the stored <paramref name="disposable"/>.</param>
    public ConditionalOneTimeDisposable(IDisposable disposable, bool closeOnDispose)
    {
        _disposable = disposable;
        _closeOnDispose = closeOnDispose;
    }

    /// <summary>Disposes the stored <see cref="IDisposable"/> if <paramref name="disposing"/> is true, also depending on the constructor's settings.</summary>
    /// <param name="disposing">Whether to dispose the stored <see cref="IDisposable"/>.</param>
    public void Dispose(bool disposing)
    {
        if (!disposing || _disposable == null)
            return;
        if (_closeOnDispose)
            _disposable.Dispose();
        _disposable = null;
    }

}
