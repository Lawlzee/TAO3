namespace TAO3.Translation;

public class Translator
{
    private readonly ITranslationService _translationService;

    public string SourceLanguage { get; }
    public string TargetLanguage { get; }

    public Translator(ITranslationService translationService, string sourceLanguage, string targetLanguage)
    {
        _translationService = translationService;
        SourceLanguage = sourceLanguage;
        TargetLanguage = targetLanguage;
    }

    public Task<string?> TranslateAsync(string text)
    {
        return _translationService.TranslateAsync(SourceLanguage, TargetLanguage, text);
    }

    public Task<string?[]> TranslateAsync(params string[] texts)
    {
        return _translationService.TranslateAsync(SourceLanguage, TargetLanguage, texts);
    }
}
