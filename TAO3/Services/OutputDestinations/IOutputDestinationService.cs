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
        Task SetTextAsync(string destination, string text, KernelInvocationContext context);
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
            _destinationByName[outputDestination.Name] = outputDestination;
            _events.OnNext(new OutputDestinationAddedEvent(outputDestination));
        }

        public Task SetTextAsync(string destination, string text, KernelInvocationContext context)
        {
            IOutputDestination? outputDestination = _destinationByName.GetValueOrDefault(destination);

            if (outputDestination == null)
            {
                throw new ArgumentException($"No output destination found for '{destination}'");
            }

            return outputDestination.SetTextAsync(text, context);
        }

        public void Dispose()
        {

        }
    }
}
