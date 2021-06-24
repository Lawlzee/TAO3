using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Cell
{
    public interface ICellService : IDisposable
    {
        IObservable<ICellEvent> Events { get; }
        NotebookCell AddOrUpdateCell(string name, string code, Kernel kernel);
        NotebookCell? TryGetCell(string name);
        bool RemoveCell(string name);
    }

    internal class CellService : ICellService
    {
        private readonly ReplaySubject<ICellEvent> _events;
        private readonly Dictionary<string, NotebookCell> _cells;

        public IObservable<ICellEvent> Events => _events;

        public CellService()
        {
            _events = new ReplaySubject<ICellEvent>();
            _cells = new Dictionary<string, NotebookCell>();
        }

        public NotebookCell AddOrUpdateCell(string name, string code, Kernel kernel)
        {
            NotebookCell? cell = _cells.GetValueOrDefault(name);
            if (cell != null)
            {
                cell.Code = code;
                cell.Kernel = kernel;
                return cell;
            }

            NotebookCell newCell = NotebookCell.Create(name, code, kernel);
            _cells.Add(name, newCell);
            _events.OnNext(new CellAddedEvent(newCell));
            return newCell;
        }

        public NotebookCell? TryGetCell(string name)
        {
            return _cells.GetValueOrDefault(name);
        }

        public bool RemoveCell(string name)
        {
            NotebookCell? cell = _cells.GetValueOrDefault(name);
            if (cell != null)
            {
                _events.OnNext(new CellRemovedEvent(cell));
                cell.Dispose();
            }
            return _cells.Remove(name);
        }

        public void Dispose()
        {
            foreach (NotebookCell cell in _cells.Values)
            {
                cell.Dispose();
            }

            _cells.Clear();
            _events.OnCompleted();
        }
    }
}
