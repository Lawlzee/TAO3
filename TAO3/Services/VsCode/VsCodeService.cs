using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.DotNet.Interactive;
using TAO3.Converters;
using TAO3.Internal.Utils;

namespace TAO3.VsCode;

public interface IVsCodeService : IDisposable
{
    Task OpenAsync();
    Task OpenAsync(string path);

    Task DiffAsync(object? left, object? right, string format = "json");
    Task DiffAsync(string left, string right, string fileExtension = ".txt");
    Task DiffFilesAsync(string path1, string path2);

    void ClearPassword();
}

internal class VsCodeService : IVsCodeService
{
    private string? _userPassword;
    private readonly IConverterService _converterService;

    public VsCodeService(IConverterService converterService)
    {  
        _converterService = converterService;
    }

    public Task OpenAsync()
    {
        return InvokeCmdAsync("code");
    }

    public Task OpenAsync(string path)
    {
        return InvokeCmdAsync(@$"code ""{path}""");
    }

    public Task DiffAsync(object? left, object? right, string format = "json")
    {
        IConverter? converter = _converterService.TryGetConverter(format);
        if (converter == null)
        {
            throw new ArgumentException($"'{format}' is not a valid format", nameof(format));
        }

        string leftJson = converter.Serialize(left);
        string rightJson = converter.Serialize(right);

        string fileExtension = converter.FileExtensions.FirstOrDefault() ?? ".txt";
        return DiffAsync(leftJson, rightJson, fileExtension);
    }

    public async Task DiffAsync(string left, string right, string fileExtension = ".txt")
    {
        using TemporaryFile leftFile = new TemporaryFile(fileExtension);
        await leftFile.WriteAsync(left);
        await leftFile.FlushAsync();

        using TemporaryFile rightFile = new TemporaryFile(fileExtension);
        await rightFile.WriteAsync(right);
        await rightFile.FlushAsync();

        await DiffFilesAsync(leftFile.Path, rightFile.Path);
    }

    public async Task DiffFilesAsync(string path1, string path2)
    {
        await InvokeCmdAsync(@$"code -d ""{path1}"" ""{path2}""");
    }

    private async Task InvokeCmdAsync(string command)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            && _userPassword == null)
        {
            _userPassword = await Kernel.GetPasswordAsync($"Password for {Environment.UserName}", "Password");
        }

        using (Process process = new Process())
        {
            try
            {
                process.StartInfo.FileName = @"cmd.exe";
                process.StartInfo.Arguments = @$"/C " + command;
                process.StartInfo.UseShellExecute = false;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    process.StartInfo.UserName = Environment.UserName;
                    process.StartInfo.Domain = Environment.UserDomainName;
                    process.StartInfo.PasswordInClearText = _userPassword;
                }

                process.Start();
                await process.WaitForExitAsync();
            }
            catch
            {
                ClearPassword();
                throw;
            }
        }
    }

    public void ClearPassword()
    {
        _userPassword = null;
    }

    public void Dispose()
    {
        ClearPassword();
    }
}
