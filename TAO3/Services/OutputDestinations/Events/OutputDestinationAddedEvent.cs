using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.OutputDestinations
{
    public class OutputDestinationAddedEvent : IOutputDestinationEvent
    {
        public IOutputDestination OutputDestination { get; }

        public OutputDestinationAddedEvent(IOutputDestination outputDestination)
        {
            OutputDestination = outputDestination;
        }
    }
}
