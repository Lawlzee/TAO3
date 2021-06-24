using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.OutputDestinations
{
    public interface IOutputDestinationService : IDisposable
    {
        IObservable<IOutputDestinationEvent> Events { get; }
        void Register(IOutputDestination outputDestination);
        bool Remove(string name);
        Task SetTextAsync(string destination, string text);
    }

    public class OutputDestinationService : IOutputDestinationService
    {
        private readonly Dictionary<string, IOutputDestination> _destinationByName;
        private readonly ReplaySubject<IOutputDestinationEvent> _events;
        public IObservable<IOutputDestinationEvent> Events => _events;

        public OutputDestinationService()
        {
            _destinationByName = new(StringComparer.OrdinalIgnoreCase);
            _events = new();
        }

        public void Register(IOutputDestination outputDestination)
        {
            if (_destinationByName.TryGetValue(outputDestination.Name, out IOutputDestination? old))
            {
                _events.OnNext(new OutputDestinationRemovedEvent(old));
            }

            _destinationByName[outputDestination.Name] = outputDestination;
            _events.OnNext(new OutputDestinationAddedEvent(outputDestination));
        }

        public bool Remove(string name)
        {
            if (_destinationByName.TryGetValue(name, out IOutputDestination? outputDestination))
            {
                _events.OnNext(new OutputDestinationRemovedEvent(outputDestination));
                _destinationByName.Remove(name);
                return true;
            }

            return false;
        }

        public Task SetTextAsync(string destination, string text)
        {
            IOutputDestination? outputDestination = _destinationByName.GetValueOrDefault(destination);

            if (outputDestination == null)
            {
                throw new ArgumentException($"No output destination found for '{destination}'");
            }

            return outputDestination.SetTextAsync(text);
        }

        public void Dispose()
        {
            _destinationByName.Clear();
            _events.Dispose();
        }
    }
}
