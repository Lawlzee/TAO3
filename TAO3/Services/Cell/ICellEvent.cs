namespace TAO3.Cell;

public interface ICellEvent
{
    public NotebookCell Cell { get; }
}

public record CellAddedEvent(NotebookCell Cell) : ICellEvent;
public record CellRemovedEvent(NotebookCell Cell) : ICellEvent;
