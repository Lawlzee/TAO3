using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TAO3.Converters;
using TAO3.Internal.Utils;

namespace TAO3.VsCode
{
    public interface IVsCodeService : IDisposable
    {
        Task OpenAsync();
        Task OpenAsync(string path);

        Task DiffAsync(object? left, object? right, string format = "json");
        Task DiffAsync(string left, string right, string fileExtension = ".txt");
        Task DiffFilesAsync(string path1, string path2);

        Task<string> AskAsync(string prompt = "", bool isPassword = false, CancellationToken cancellationToken = default);

        void ClearPassword();
    }

    internal class VsCodeService : IVsCodeService
    {
        private string? _userPassword;
        //private readonly IInteractiveHost _interactiveHost;
        private readonly IConverterService _converterService;

        public VsCodeService(/*IInteractiveHost interactiveHost, */IConverterService converterService)
        {  
            //_interactiveHost = interactiveHost;
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
                _userPassword = await AskAsync($"Password for {Environment.UserName}", isPassword: true);
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

        public Task<string> AskAsync(string prompt = "", bool isPassword = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //return _interactiveHost.GetInputAsync(prompt, isPassword, cancellationToken);
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
}
