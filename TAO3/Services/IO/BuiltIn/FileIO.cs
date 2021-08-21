using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Commands;

namespace TAO3.IO
{
    internal class FileOptions
    {
        public string Path { get; set; } = null!;
        public Encoding? Encoding { get; set; }
    }

    internal class FileIO 
        : ISource<FileOptions>, 
        IDestination<FileOptions>,
        IConfigurableSource,
        IConfigurableDestination
    {
        public string Name => "file";
        public IReadOnlyList<string> Aliases => new[] { "File" };

        public void Configure(Command command)
        {
            command.Add(CommandFactory.CreatePathArgument("path"));

            Option<Encoding?> encodingOptions = new Option<Encoding?>("encoding", result =>
            {
                if (result.Tokens.Count == 0)
                {
                    return null;
                }

                string encodingName = result.Tokens[0].Value;
                EncodingInfo? encodingInfo = Encoding.GetEncodings()
                    .FirstOrDefault(x => x.Name == encodingName);

                if (encodingInfo == null)
                {
                    result.ErrorMessage = $"The encoding '{encodingName}' is invalid";
                    return null;
                }

                return encodingInfo.GetEncoding();
            });

            encodingOptions.AddSuggestions(Encoding
                .GetEncodings()
                .Select(x => x.Name)
                .ToArray());

            command.Add(encodingOptions);
        }

        public Task<string> GetTextAsync(FileOptions options)
        {
            if (options.Encoding != null)
            {
                return File.ReadAllTextAsync(options.Path, options.Encoding);
            }

            return File.ReadAllTextAsync(options.Path);
        }

        public Task SetTextAsync(string text, FileOptions options)
        {
            if (options.Encoding != null)
            {
                return File.WriteAllTextAsync(options.Path, text, options.Encoding);
            }

            return File.WriteAllTextAsync(options.Path, text);
        }
    }
}
