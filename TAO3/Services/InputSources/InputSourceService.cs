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
        IObservable<InputSourceAddedEvent> Events { get; }
        
        Task<string> GetTextAsync(string source, KernelInvocationContext context);
        void Register(IInputSource inputSource);
        void Register(string name, Func<KernelInvocationContext, Task<string>> getText);
        void Register(string name, Func<KernelInvocationContext, string> getText);
        void RegisterFile(string name, string path, Encoding? encoding = null);
        void RegisterUri(string name, string uri);
    }

    public class InputSourceService : IInputSourceService
    {
        private readonly HttpClient _httpClient;

        private readonly Dictionary<string, IInputSource> _sourceByName;
        private readonly Subject<InputSourceAddedEvent> _events;
        public IObservable<InputSourceAddedEvent> Events => _events;

        public InputSourceService()
        {
            _httpClient = new();
            _sourceByName = new(StringComparer.OrdinalIgnoreCase);
            _events = new();
        }

        public void Register(IInputSource inputSource)
        {
            _sourceByName[inputSource.Name] = inputSource;
            _events.OnNext(new InputSourceAddedEvent(inputSource));
        }

        public Task<string> GetTextAsync(string source, KernelInvocationContext context)
        {
            IInputSource? inputSource = _sourceByName.GetValueOrDefault(source);

            if (inputSource == null)
            {
                throw new ArgumentException($"No input source found for '{source}'");
            }

            return inputSource.GetTextAsync(context);
        }

        public void RegisterFile(string name, string path, Encoding? encoding = null)
        {
            Register(name, c => File.ReadAllTextAsync(path, encoding ?? Encoding.UTF8));
        }

        public void RegisterUri(string name, string uri)
        {
            Register(name, async c => await _httpClient.GetStringAsync(uri));
        }

        public void Register(string name, Func<KernelInvocationContext, Task<string>> getText)
        {
            Register(new CustomInputSource(name, getText));
        }

        public void Register(string name, Func<KernelInvocationContext, string> getText)
        {
            Register(new CustomInputSource(name, c => Task.Run(() => getText(c))));
        }

        private class CustomInputSource : IInputSource
        {
            private readonly Func<KernelInvocationContext, Task<string>> _getText;
            public string Name { get; }

            public CustomInputSource(string name, Func<KernelInvocationContext, Task<string>> getText)
            {
                Name = name;
                _getText = getText;
            }

            public Task<string> GetTextAsync(KernelInvocationContext context)
            {
                return _getText(context);
            }

            public override string? ToString()
            {
                return Name;
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _events.Dispose();
        }
    }
}
