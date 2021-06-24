using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Cell
{
    public class CellAddedEvent : ICellEvent
    {
        public NotebookCell Cell { get; }

        public CellAddedEvent(NotebookCell cell)
        {
            Cell = cell;
        }
    }
}
