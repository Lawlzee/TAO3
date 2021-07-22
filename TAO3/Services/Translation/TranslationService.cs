using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Translation
{
    public interface ITranslationService : IDisposable
    {
        void Configure(string url, string? apiKey = null);

        Translator CreateTranslator(string sourceLanguage, string targetLanguage);
        Translator CreateTranslator(Language sourceLanguage, Language targetLanguage);

        Task<string?> TranslateAsync(string sourceLanguage, string targetLanguage, string text);
        Task<string?> TranslateAsync(Language sourceLanguage, Language targetLanguage, string text);

        Task<string?[]> TranslateAsync(string sourceLanguage, string targetLanguage, params string[] texts);
        Task<string?[]> TranslateAsync(Language sourceLanguage, Language targetLanguage, params string[] texts);
    }

    public class TranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        private string? _url;
        private string? _apiKey;

        public TranslationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void Configure(string url, string? apiKey = null)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
            _apiKey = apiKey;
        }

        public Translator CreateTranslator(string sourceLanguage, string targetLanguage)
        {
            return new Translator(this, sourceLanguage, targetLanguage);
        }

        public Translator CreateTranslator(Language sourceLanguage, Language targetLanguage)
        {
            return new Translator(this, sourceLanguage.ToString(), targetLanguage.ToString());
        }

        public Task<string?> TranslateAsync(Language sourceLanguage, Language targetLanguage, string text)
        {
            return TranslateAsync(sourceLanguage.ToString(), targetLanguage.ToString(), text);
        }

        public async Task<string?> TranslateAsync(string sourceLanguage, string targetLanguage, string text)
        {
            if (_url == null)
            {
                throw new Exception("TranslationService is not configured. Please call ITranslationService.Configure(string url, string? apiKey = null).");
            }

            var formUrlEncodedContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string?, string?>("q", text),
                new KeyValuePair<string?, string?>("source", sourceLanguage),
                new KeyValuePair<string?, string?>("target", targetLanguage),
                new KeyValuePair<string?, string?>("api_key", _apiKey)
            });

            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, _url + "/translate")
            {
                Content = formUrlEncodedContent
            });

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()).translatedText;
            }

            return null;
        }

        public Task<string?[]> TranslateAsync(Language sourceLanguage, Language targetLanguage, params string[] texts)
        {
            return TranslateAsync(sourceLanguage.ToString(), targetLanguage.ToString(), texts);
        }

        public async Task<string?[]> TranslateAsync(string sourceLanguage, string targetLanguage, params string[] texts)
        {
            Task<string?>[] translations = texts
               .Select(text => TranslateAsync(sourceLanguage, targetLanguage, text))
               .ToArray();

            await Task.WhenAll(translations);

            return translations
                .Select(x => x.Result)
                .ToArray();
        }

        public void Dispose()
        {
            
        }
    }
}
