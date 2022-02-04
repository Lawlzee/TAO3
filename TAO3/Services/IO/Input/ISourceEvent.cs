namespace TAO3.IO;

public interface ISourceEvent
{
    ISource Source { get; }
}

public record SourceAddedEvent(ISource Source) : ISourceEvent;

public record SourceRemovedEvent(ISource Source) : ISourceEvent;
