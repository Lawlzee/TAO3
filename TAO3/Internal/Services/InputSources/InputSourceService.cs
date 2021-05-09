using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using TAO3.InputSources;

namespace TAO3.Internal.Services
{
    internal interface IInputSourceService
    {
        IObservable<InputSourceAddedEvent> Events { get; }
        void Register(IInputSource inputSource);
        Task<string> GetTextAsync(string source, KernelInvocationContext context);
    }

    internal class InputSourceService : IInputSourceService
    {
        private readonly List<IInputSource> _sources;
        private readonly Subject<InputSourceAddedEvent> _events;
        public IObservable<InputSourceAddedEvent> Events => _events;

        public InputSourceService()
        {
            _sources = new List<IInputSource>();
            _events = new();
        }

        public void Register(IInputSource inputSource)
        {
            _sources.Add(inputSource);
            _events.OnNext(new InputSourceAddedEvent(inputSource));
        }

        public Task<string> GetTextAsync(string source, KernelInvocationContext context)
        {
            Task<string>? task = _sources
                .Where(inputSource => source.StartsWith(inputSource.Name, StringComparison.OrdinalIgnoreCase))
                .Where(inputSource => source.Length == inputSource.Name.Length || source[inputSource.Name.Length] == ':')
                .Select(inputSource => inputSource.GetText(source.Substring(source.Length == inputSource.Name.Length ? inputSource.Name.Length : inputSource.Name.Length + 1), context))
                .FirstOrDefault()!;

            if (task == null)
            {
                throw new ArgumentException($"No input source found for '{source}'");
            }

            return task;
        }
    }
}
