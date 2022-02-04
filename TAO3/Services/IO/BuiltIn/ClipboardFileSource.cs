using System.CommandLine;
using System.IO;
using System.Reflection;
using TAO3.Clipboard;
using TAO3.Internal.Commands;
using TAO3.TypeProvider;

namespace TAO3.IO;

public record ClipboardFileOptions
{
    public Encoding? Encoding { get; init; }
}

internal class ClipboardFileSource :
    IIntermediateSource<ClipboardFileOptions>,
    IConfigurableSource
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

    public async Task<IDomType> ProvideTypeAsync(ClipboardFileOptions options, Func<ProvideSourceArguments, Task<IDomType>> inferChildTypeAsync)
    {
        List<string> files = await _clipboardService.GetFilesAsync();

        //if (files.Count == 0)
        //{
        //    throw new Exception("No file found in clipboard");
        //}

        //todo: change
        string className = "Temp";

        var fileContents = await Task.WhenAll(files
            .Select(async path => new
            {
                Path = path,
                Text = options.Encoding != null
                    ? await File.ReadAllTextAsync(path, options.Encoding)
                    : await File.ReadAllTextAsync(path)
            }));

        DomClassProperty[] properties = await Task.WhenAll(fileContents
            .Select(async x => new DomClassProperty(
                identifier: Path.GetFileNameWithoutExtension(x.Path),
                name: Path.GetFileName(x.Path),
                await inferChildTypeAsync(new ProvideSourceArguments(x.Path, x.Text)))));

        return new DomClass(
            className,
            properties.ToList());
    }

    public async Task<T> GetAsync<T>(ClipboardFileOptions options, Func<DeserializeArguments, object?> deserializeChild)
    {
        List<string> files = await _clipboardService.GetFilesAsync();

        T result = Activator.CreateInstance<T>();
        foreach ((PropertyInfo property, string path) in typeof(T).GetProperties().Zip(files))
        {
            object? value = deserializeChild(new DeserializeArguments(path, property.PropertyType));

            property.SetValue(result, value);
        }

        return result;
    }
}
