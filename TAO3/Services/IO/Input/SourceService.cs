using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using TAO3.IO;

namespace TAO3.IO
{
    public interface ISourceService : IDisposable
    {
        IObservable<ISourceEvent> Events { get; }
        
        Task<string> GetTextAsync(string name);
        
        void Register(ISource source);
        void Register(string name, Func<Task<string>> getText, IReadOnlyList<string>? aliases = null);
        void Register(string name, Func<string> getText, IReadOnlyList<string>? aliases = null);
        void RegisterFile(string name, string path, Encoding? encoding = null);
        void RegisterUri(string name, string uri);

        bool Remove(string name);
    }

    public class SourceService : ISourceService
    {
        private readonly HttpClient _httpClient;

        private readonly Dictionary<string, ISource> _sourceByName;
        private readonly ReplaySubject<ISourceEvent> _events;
        public IObservable<ISourceEvent> Events => _events;

        public SourceService()
        {
            _httpClient = new();
            _sourceByName = new(StringComparer.OrdinalIgnoreCase);
            _events = new();
        }

        public void Register(ISource source)
        {
            if (_sourceByName.TryGetValue(source.Name, out ISource? oldInputSource))
            {
                _events.OnNext(new SourceRemovedEvent(oldInputSource));
            }

            _sourceByName[source.Name] = source;
            _events.OnNext(new SourceAddedEvent(source));
        }

        public Task<string> GetTextAsync(string name)
        {
            ISource? source = _sourceByName.GetValueOrDefault(name);

            if (source == null)
            {
                throw new ArgumentException($"No input source found for '{name}'");
            }

            return source.GetTextAsync();
        }

        public void RegisterFile(string name, string path, Encoding? encoding = null)
        {
            Register(name, () => File.ReadAllTextAsync(path, encoding ?? Encoding.UTF8));
        }

        public void RegisterUri(string name, string uri)
        {
            Register(name, async () => await _httpClient.GetStringAsync(uri));
        }

        public void Register(string name, Func<Task<string>> getText, IReadOnlyList<string>? aliases = null)
        {
            Register(new CustomInputSource(
                name, 
                getText,
                aliases ?? Array.Empty<string>()));
        }

        public void Register(string name, Func<string> getText, IReadOnlyList<string>? aliases = null)
        {
            Register(new CustomInputSource(
                name, 
                () => Task.Run(() => getText()),
                aliases ?? Array.Empty<string>()));
        }

        public bool Remove(string name)
        {
            if (_sourceByName.TryGetValue(name, out var source))
            {
                _sourceByName.Remove(name);
                _events.OnNext(new SourceRemovedEvent(source));
                return true;
            }

            return false;
        }

        private class CustomInputSource : ISource
        {
            private readonly Func<Task<string>> _getText;
            public string Name { get; }
            public IReadOnlyList<string> Aliases { get; }

            public CustomInputSource(string name, Func<Task<string>> getText, IReadOnlyList<string> aliases)
            {
                Name = name;
                _getText = getText;
                Aliases = aliases;
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
