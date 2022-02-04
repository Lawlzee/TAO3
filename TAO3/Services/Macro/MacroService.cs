using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Events;
using System.Diagnostics;
using System.Reactive.Subjects;
using TAO3.Internal.Extensions;
using TAO3.Keyboard;

namespace TAO3.Macro;

public interface IMacroService : IDisposable
{
    IReadOnlyList<TAO3Macro> Macros { get; }
    IObservable<IMacroEvent> Events { get; }

    void Add(TAO3Macro macro);
    void Remove(TAO3Macro macro);
}

internal class MacroService : IMacroService
{
    private readonly IKeyboardService _keyboardService;
    private readonly CompositeKernel _compositeKernel;

    private readonly List<TAO3Macro> _macros;
    public IReadOnlyList<TAO3Macro> Macros => _macros;

    private readonly Subject<IMacroEvent> _events;
    public IObservable<IMacroEvent> Events => _events;

    public MacroService(IKeyboardService keyboardService, CompositeKernel compositeKernel)
    {
        _keyboardService = keyboardService;
        _compositeKernel = compositeKernel;
        _macros = new List<TAO3Macro>();
        _events = new Subject<IMacroEvent>();
    }

    public void Add(TAO3Macro macro)
    {
         List<TAO3Macro> macroToRemove = _macros
            .Where(x => x.Name == macro.Name || x.Shortcut == macro.Shortcut)
            .ToList();

        foreach (TAO3Macro m in macroToRemove)
        {
            Remove(m);
        }

        _keyboardService.RegisterOnKeyPressed(macro.Shortcut, () => _ = RunMacroAsync(macro));
        _macros.Add(macro);
        _events.OnNext(new MacroAdded(macro));
    }

    private async Task RunMacroAsync(TAO3Macro macro)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        SubmitCode submitCode = new SubmitCode(macro.Code, macro.TargetKernelName);

        IDisposable? subscription = null;
        try
        {
            subscription = _compositeKernel.KernelEvents.Subscribe(
                onNext: e =>
                {
                    KernelCommand rootCommand = e.Command.GetRootCommand();
                    if (rootCommand == submitCode)
                    {
                        if (e is CommandFailed failed)
                        {
                            _events.OnNext(new MacroExecutionFailed(macro, failed));
                        }

                        if (e is ReturnValueProduced valueProduced)
                        {
                            _events.OnNext(new MacroValueProduced(macro, valueProduced));
                        }
                    }
                });

            await _compositeKernel.SendAsync(submitCode);
        }
        catch (Exception e)
        {
            _events.OnNext(new MacroExecutionFailed(macro, new CommandFailed(e, submitCode)));
        }
        finally
        {
            _events.OnNext(new MacroExecutionCompleted(macro, stopwatch.Elapsed));
            subscription?.Dispose();
            stopwatch.Stop();
        }
    }

    public void Remove(TAO3Macro macro)
    {
        _macros.Remove(macro);
        _events.OnNext(new MacroRemoved(macro));
    }


    public void Dispose()
    {
        foreach (TAO3Macro macro in _macros.ToList())
        {
            Remove(macro);
        }

        _events.Dispose();
    }
}
