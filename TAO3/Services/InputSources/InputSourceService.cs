using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using TAO3.InputSources;

namespace TAO3.InputSources
{
    public interface IInputSourceService : IDisposable
    {
        IObservable<IInputSourceEvent> Events { get; }
        
        Task<string> GetTextAsync(string source);
        
        void Register(IInputSource inputSource);
        void Register(string name, Func<Task<string>> getText);
        void Register(string name, Func<string> getText);
        void RegisterFile(string name, string path, Encoding? encoding = null);
        void RegisterUri(string name, string uri);

        bool Remove(string name);
    }

    public class InputSourceService : IInputSourceService
    {
        private readonly HttpClient _httpClient;

        private readonly Dictionary<string, IInputSource> _sourceByName;
        private readonly ReplaySubject<IInputSourceEvent> _events;
        public IObservable<IInputSourceEvent> Events => _events;

        public InputSourceService()
        {
            _httpClient = new();
            _sourceByName = new(StringComparer.OrdinalIgnoreCase);
            _events = new();
        }

        public void Register(IInputSource inputSource)
        {
            if (_sourceByName.TryGetValue(inputSource.Name, out IInputSource? oldInputSource))
            {
                _events.OnNext(new InputSourceRemovedEvent(oldInputSource));
            }

            _sourceByName[inputSource.Name] = inputSource;
            _events.OnNext(new InputSourceAddedEvent(inputSource));
        }

        public Task<string> GetTextAsync(string source)
        {
            IInputSource? inputSource = _sourceByName.GetValueOrDefault(source);

            if (inputSource == null)
            {
                throw new ArgumentException($"No input source found for '{source}'");
            }

            return inputSource.GetTextAsync();
        }

        public void RegisterFile(string name, string path, Encoding? encoding = null)
        {
            Register(name, () => File.ReadAllTextAsync(path, encoding ?? Encoding.UTF8));
        }

        public void RegisterUri(string name, string uri)
        {
            Register(name, async () => await _httpClient.GetStringAsync(uri));
        }

        public void Register(string name, Func<Task<string>> getText)
        {
            Register(new CustomInputSource(name, getText));
        }

        public void Register(string name, Func<string> getText)
        {
            Register(new CustomInputSource(name, () => Task.Run(() => getText())));
        }

        public bool Remove(string name)
        {
            if (_sourceByName.TryGetValue(name, out var source))
            {
                _sourceByName.Remove(name);
                _events.OnNext(new InputSourceRemovedEvent(source));
                return true;
            }

            return false;
        }

        private class CustomInputSource : IInputSource
        {
            private readonly Func<Task<string>> _getText;
            public string Name { get; }

            public CustomInputSource(string name, Func<Task<string>> getText)
            {
                Name = name;
                _getText = getText;
            }

            public Task<string> GetTextAsync()
            {
                return _getText();
            }

            public override string? ToString()
            {
                return Name;
            }
        }

        public void Dispose()
        {
            _sourceByName.Clear();
            _httpClient.Dispose();
            _events.Dispose();
        }
    }
}
