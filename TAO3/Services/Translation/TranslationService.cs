using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TAO3.NaturalLanguage;

namespace TAO3.Translation
{
    public interface ITranslationService : IDisposable
    {
        void Configure(string url, string? apiKey = null);

        Task<string?> TranslateAsync(string sourceLanguage, string targetLanguage, string text);
        Task<string?> TranslateAsync(Language sourceLanguage, Language targetLanguage, string text);

        Task<string?[]> TranslateAsync(string sourceLanguage, string targetLanguage, params string[] texts);
        Task<string?[]> TranslateAsync(Language sourceLanguage, Language targetLanguage, params string[] texts);

        ILanguageDictionary? GetDictionary(string language);
        ILanguageDictionary? GetDictionary(Language language);


        void LoadDictionary(string language, ILanguageDictionary dictionary);
        void LoadDictionary(Language language, ILanguageDictionary dictionary);
    }

    public class TranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, ILanguageDictionary> _dictionaryByLanguage;
        private string? _url;
        private string? _apiKey;

        public TranslationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _dictionaryByLanguage = new Dictionary<string, ILanguageDictionary>();
        }

        public void Configure(string url, string? apiKey = null)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
            _apiKey = apiKey;
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

        public ILanguageDictionary? GetDictionary(string language)
        {
            return _dictionaryByLanguage.GetValueOrDefault(language);
        }

        public ILanguageDictionary? GetDictionary(Language language)
        {
            return _dictionaryByLanguage.GetValueOrDefault(language.ToString());
        }

        public void LoadDictionary(string language, ILanguageDictionary dictionary)
        {
            _dictionaryByLanguage[language] = dictionary;
        }

        public void LoadDictionary(Language language, ILanguageDictionary dictionary)
        {
            _dictionaryByLanguage[language.ToString()] = dictionary;
        }

        public void Dispose()
        {
            
        }

        
    }
}
