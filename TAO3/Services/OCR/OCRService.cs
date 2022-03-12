using System.Drawing;
using System.IO;
using System.Net.Http;
using Tesseract;

namespace TAO3.Services;

public interface IOCRService : IDisposable
{
    Uri BaseTraindedDataUri { get; set; }
    string DataPath { get; }

    Task<string> GetTextAsync(OCRLanguage language, Image image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null);
    Task<string> GetTextAsync(string language, Image image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null);

    Task<string> GetTextAsync(OCRLanguage language, string path, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null);
    Task<string> GetTextAsync(string language, string path, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null);

    Task<string> GetTextAsync(OCRLanguage language, Pix image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null);
    Task<string> GetTextAsync(string language, Pix image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null);

    Task<Page> ProcessAsync(OCRLanguage language, Image image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null);
    Task<Page> ProcessAsync(string language, Image image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null);

    Task<Page> ProcessAsync(OCRLanguage language, string path, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null);
    Task<Page> ProcessAsync(string language, string path, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null);

    Task<Page> ProcessAsync(OCRLanguage language, Pix image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null);
    Task<Page> ProcessAsync(string language, Pix image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null);

    Task<TesseractEngine> CreateEngineAsync(OCRLanguage language);
    Task<TesseractEngine> CreateEngineAsync(string language);

    Task LoadLanguageAsync(string language);
    Task LoadLanguageAsync(OCRLanguage language);

    bool IsLanguageLoaded(string language);
    bool IsLanguageLoaded(OCRLanguage language);


}

internal class OCRService : IOCRService
{
    private readonly HttpClient _httpClient;

    public Uri BaseTraindedDataUri { get; set; }
    public string DataPath { get; }

    public OCRService(HttpClient httpClient)
    {
        DataPath = Path.GetTempPath();
        BaseTraindedDataUri = new Uri("https://raw.githubusercontent.com/tesseract-ocr/tessdata_fast/main");
        _httpClient = httpClient;
    }

    public async Task<string> GetTextAsync(OCRLanguage language, Image image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null)
    {
        using Page page = await ProcessAsync(language, image, inputName, region, pageSegMode);
        return page.GetText();
    }

    public async Task<string> GetTextAsync(string language, Image image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null)
    {
        using Page page = await ProcessAsync(language, image, inputName, region, pageSegMode);
        return page.GetText();
    }

    public async Task<string> GetTextAsync(OCRLanguage language, string path, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null)
    {
        using Page page = await ProcessAsync(language, path, inputName, region, pageSegMode);
        return page.GetText();
    }

    public async Task<string> GetTextAsync(string language, string path, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null)
    {
        using Page page = await ProcessAsync(language, path, inputName, region, pageSegMode);
        return page.GetText();
    }

    public async Task<string> GetTextAsync(OCRLanguage language, Pix image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null)
    {
        using Page page = await ProcessAsync(language, image, inputName, region, pageSegMode);
        return page.GetText();
    }

    public async Task<string> GetTextAsync(string language, Pix image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null)
    {
        using Page page = await ProcessAsync(language, image, inputName, region, pageSegMode);
        return page.GetText();
    }

    public async Task<Page> ProcessAsync(OCRLanguage language, Image image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null)
    {
        return await ProcessAsync(language.ToString(), image, inputName, region, pageSegMode);
    }

    public async Task<Page> ProcessAsync(string language, Image image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null)
    {
        using var img = PixConverter.ToPix((Bitmap)image);
        return await ProcessAsync(language, img);
    }

    public async Task<Page> ProcessAsync(OCRLanguage language, string path, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null)
    {
        return await ProcessAsync(language.ToString(), path, inputName, region, pageSegMode);
    }

    public async Task<Page> ProcessAsync(string language, string path, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null)
    {
        using var img = Pix.LoadFromFile(path);
        return await ProcessAsync(language, img);
    }

    public async Task<Page> ProcessAsync(OCRLanguage language, Pix image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null)
    {
        return await ProcessAsync(language.ToString(), image, inputName, region, pageSegMode);
    }

    public async Task<Page> ProcessAsync(string language, Pix image, string? inputName = null, Rect? region = null, PageSegMode? pageSegMode = null)
    {
        //todo: cache?
        TesseractEngine engine = await CreateEngineAsync(language);
        return engine.Process(image, inputName, region ?? new Rect(0, 0, image.Width, image.Height), pageSegMode);
    }

    public async Task<TesseractEngine> CreateEngineAsync(OCRLanguage language)
    {
        return await CreateEngineAsync(language.ToString());
    }

    public async Task<TesseractEngine> CreateEngineAsync(string language)
    {
        await LoadLanguageAsync(language);
        return new TesseractEngine(DataPath, language);
    }

    public async Task LoadLanguageAsync(OCRLanguage language)
    {
        await LoadLanguageAsync(language.ToString());
    }

    public async Task LoadLanguageAsync(string language)
    {
        if (!IsLanguageLoaded(language))
        {
            string fileName = language + ".traineddata";
            string filePath = Path.Combine(DataPath, fileName);

            Uri uri = new Uri(BaseTraindedDataUri, fileName);
            HttpResponseMessage response = await _httpClient.GetAsync(uri);

            using FileStream fs = new FileStream(filePath, FileMode.Create);
            await response.Content.CopyToAsync(fs);
        }
    }

    public bool IsLanguageLoaded(OCRLanguage language)
    {
        return IsLanguageLoaded(language.ToString());
    }

    public bool IsLanguageLoaded(string language)
    {
        string fileName = language + ".traineddata";
        string filePath = Path.Combine(DataPath, fileName);
        return File.Exists(filePath);
    }

    public void Dispose()
    {

    }
}
