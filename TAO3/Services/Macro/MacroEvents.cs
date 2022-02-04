using Microsoft.DotNet.Interactive.Events;

namespace TAO3.Macro;

public interface IMacroEvent
{
    TAO3Macro Macro { get; }
}

public record MacroAdded(TAO3Macro Macro) : IMacroEvent;
public record MacroRemoved(TAO3Macro Macro) : IMacroEvent;

public interface IMacroExecutionEvent : IMacroEvent
{
    KernelEvent Result { get; }
    TimeSpan ExecutionTime { get; }
}

public record MacroValueProduced(
    TAO3Macro Macro, 
    ReturnValueProduced ReturnValueProduced) : IMacroEvent;

public record MacroExecutionFailed(
    TAO3Macro Macro,
    CommandFailed CommandFailed) : IMacroEvent;

public record MacroExecutionCompleted(
    TAO3Macro Macro, 
    TimeSpan ExecutionTime) : IMacroEvent;
