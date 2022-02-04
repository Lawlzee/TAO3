using System.CommandLine;
using TAO3.Converters;
using TAO3.Notepad;

namespace TAO3.IO;

internal record NotepadDestinationOptions
{
    public NppLanguage? Language { get; init; }
    public IConverter Converter { get; init; } = null!;
}

internal class NotepadIO : 
    ITextSource, 
    IDestination<NotepadDestinationOptions>,
    IConfigurableDestination
{
    private readonly INotepadService _notepad;
    public string Name => "notepad++";

    public IReadOnlyList<string> Aliases => new[] { "notepad", "npp", "n++" };

    public NotepadIO(INotepadService notepad)
    {
        _notepad = notepad;
    }

    public Task<string> GetTextAsync()
    {
        return Task.Run(_notepad.GetText);
    }

    public void Configure(Command command)
    {
        command.Add(new Option<NppLanguage?>("--language", "-l"));
    }

    public Task SetTextAsync(string text, NotepadDestinationOptions options)
    {
        _notepad.SetText(text);
        _notepad.SetLanguage(GetLanguage(options));

        return Task.CompletedTask;
    }

    private NppLanguage GetLanguage(NotepadDestinationOptions options)
    {
        if (options.Language != null)
        {
            return options.Language.Value;
        }
        
        if (options.Converter.Properties.TryGetValue(nameof(NppLanguage), out object? language)
            && language is NppLanguage nppLanguage)
        {
            return nppLanguage;
        }
        
        if (Enum.TryParse(options.Converter.Format, ignoreCase: true, out NppLanguage inferedLanguage))
        {
            return inferedLanguage;
        }

        return NppLanguage.TEXT;
    }
}
