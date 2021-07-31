using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.IO
{
    public interface IDestinationService : IDisposable
    {
        IObservable<IDestinationEvent> Events { get; }
        void Register(IDestination destination);
        bool Remove(string name);
        Task SetTextAsync(string name, string text);
    }

    public class DestinationService : IDestinationService
    {
        private readonly Dictionary<string, IDestination> _destinationByName;
        private readonly ReplaySubject<IDestinationEvent> _events;
        public IObservable<IDestinationEvent> Events => _events;

        public DestinationService()
        {
            _destinationByName = new(StringComparer.OrdinalIgnoreCase);
            _events = new();
        }

        public void Register(IDestination destination)
        {
            if (_destinationByName.TryGetValue(destination.Name, out IDestination? old))
            {
                _events.OnNext(new DestinationRemovedEvent(old));
            }

            _destinationByName[destination.Name] = destination;
            _events.OnNext(new DestinationAddedEvent(destination));
        }

        public bool Remove(string name)
        {
            if (_destinationByName.TryGetValue(name, out IDestination? outputDestination))
            {
                _events.OnNext(new DestinationRemovedEvent(outputDestination));
                _destinationByName.Remove(name);
                return true;
            }

            return false;
        }

        public Task SetTextAsync(string name, string text)
        {
            IDestination? destination = _destinationByName.GetValueOrDefault(name);

            if (destination == null)
            {
                throw new ArgumentException($"No output destination found for '{name}'");
            }

            return destination.SetTextAsync(text);
        }

        public void Dispose()
        {
            _destinationByName.Clear();
            _events.Dispose();
        }
    }
}
