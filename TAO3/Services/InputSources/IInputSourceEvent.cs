using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.InputSources
{
    public interface IInputSourceEvent
    {
        IInputSource InputSource { get; }
    }

    public record InputSourceAddedEvent(IInputSource InputSource) : IInputSourceEvent;

    public record InputSourceRemovedEvent(IInputSource InputSource) : IInputSourceEvent;
}
