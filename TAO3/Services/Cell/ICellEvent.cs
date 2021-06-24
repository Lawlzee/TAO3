using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Cell
{
    public interface ICellEvent
    {
        public NotebookCell Cell { get; }
    }

    public record CellAddedEvent(NotebookCell Cell) : ICellEvent;
    public record CellRemovedEvent(NotebookCell Cell) : ICellEvent;
}
