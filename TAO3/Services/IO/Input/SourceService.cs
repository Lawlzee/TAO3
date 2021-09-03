using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using TAO3.IO;

namespace TAO3.IO
{
    public interface ISourceService : IDisposable
    {
        IObservable<ISourceEvent> Events { get; }
        IEnumerable<ISource> Sources { get; }

        void Register<T>(ITextSource<T> source);
        void Register<T>(IIntermediateSource<T> source);
        bool Remove(string name);
    }

    public class SourceService : ISourceService
    {
        private readonly Dictionary<string, ISource> _sourceByName;
        private readonly ReplaySubject<ISourceEvent> _events;
        public IObservable<ISourceEvent> Events => _events;

        public IEnumerable<ISource> Sources => _sourceByName.Values;

        public SourceService()
        {
            _sourceByName = new(StringComparer.OrdinalIgnoreCase);
            _events = new();
        }

        public void Register<T>(ITextSource<T> source)
        {
            DoRegister(source);
        }

        public void Register<T>(IIntermediateSource<T> source)
        {
            DoRegister(source);
        }

        private void DoRegister(ISource source)
        {
            if (_sourceByName.TryGetValue(source.Name, out ISource? oldInputSource))
            {
                _events.OnNext(new SourceRemovedEvent(oldInputSource));
            }

            _sourceByName[source.Name] = source;
            _events.OnNext(new SourceAddedEvent(source));
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

        public void Dispose()
        {
            _sourceByName.Clear();
            _events.Dispose();
        }
    }
}
