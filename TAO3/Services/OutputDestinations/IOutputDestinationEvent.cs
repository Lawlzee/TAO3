using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.OutputDestinations
{
    public interface IOutputDestinationEvent
    {
        IOutputDestination OutputDestination { get; }
    }

    public record OutputDestinationAddedEvent(IOutputDestination OutputDestination) : IOutputDestinationEvent;
    public record OutputDestinationRemovedEvent(IOutputDestination OutputDestination) : IOutputDestinationEvent;
}
