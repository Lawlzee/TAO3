using System.IO;

namespace TAO3.Internal.Utils;

internal class TemporaryFile : IDisposable
{
    private readonly FileStream _fileStream;
    public string Path { get; }

    public TemporaryFile(string extension)
    {
        Path = System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), extension);
        _fileStream = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose | FileOptions.Asynchronous);
    }

    public Task WriteAsync(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        return _fileStream.WriteAsync(bytes, 0, bytes.Length);
    }

    public Task FlushAsync()
    {
        return _fileStream.FlushAsync();
    }

    public void Dispose()
    {
        _fileStream.Dispose();
    }
}
