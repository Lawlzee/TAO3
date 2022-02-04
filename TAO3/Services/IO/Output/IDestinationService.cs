using System.Reactive.Subjects;

namespace TAO3.IO;

public interface IDestinationService : IDisposable
{
    IObservable<IDestinationEvent> Events { get; }
    IEnumerable<IDestination> Destinations {  get; }

    void Register<T>(IDestination<T> destination);
    bool Remove(string name);
}

public class DestinationService : IDestinationService
{
    private readonly Dictionary<string, IDestination> _destinationByName;
    private readonly ReplaySubject<IDestinationEvent> _events;
    public IObservable<IDestinationEvent> Events => _events;

    public IEnumerable<IDestination> Destinations => _destinationByName.Values;

    public DestinationService()
    {
        _destinationByName = new(StringComparer.OrdinalIgnoreCase);
        _events = new();
    }

    public void Register<T>(IDestination<T> destination)
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

    public void Dispose()
    {
        _destinationByName.Clear();
        _events.Dispose();
    }
}
