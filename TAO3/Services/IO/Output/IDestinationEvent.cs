namespace TAO3.IO;

public interface IDestinationEvent
{
    IDestination Destination { get; }
}

public record DestinationAddedEvent(IDestination Destination) : IDestinationEvent;
public record DestinationRemovedEvent(IDestination Destination) : IDestinationEvent;
