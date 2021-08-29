using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TAO3.Clipboard;
using TAO3.Internal.Commands;

namespace TAO3.IO
{
    internal record ClipboardFileOptions
    {
        public Encoding? Encoding { get; init; }
    }

    internal class ClipboardFileSource : ISource<ClipboardFileOptions>, IConfigurableSource
    {
        private readonly IClipboardService _clipboardService;

        public string Name => "clipboardFile";
        public IReadOnlyList<string> Aliases => new[] { "cbf" };

        public ClipboardFileSource(IClipboardService clipboardService)
        {
            _clipboardService = clipboardService;
        }

        public void Configure(Command command)
        {
            command.Add(CommandFactory.CreateEncodingOptions());
        }

        public async Task<string> GetTextAsync(ClipboardFileOptions options)
        {
            List<string> files = await _clipboardService.GetFilesAsync();
            if (files.Count == 0)
            {
                throw new Exception("No file found in clipboard");
            }

            if (files.Count > 1)
            {
                throw new NotImplementedException("Support for multiple file in clipboard is not implemented yet.");
            }

            string path = files[0];

            if (options.Encoding != null)
            {
                return await File.ReadAllTextAsync(path, options.Encoding);
            }

            return await File.ReadAllTextAsync(path);
        }
    }
}
