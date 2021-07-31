using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.IO
{
    public interface ISourceEvent
    {
        ISource Source { get; }
    }

    public record SourceAddedEvent(ISource Source) : ISourceEvent;

    public record SourceRemovedEvent(ISource Source) : ISourceEvent;
}
