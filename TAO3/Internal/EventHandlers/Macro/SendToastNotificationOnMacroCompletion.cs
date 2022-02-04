using Microsoft.DotNet.Interactive.Events;
using System.Reactive.Linq;
using TAO3.Macro;
using TAO3.Toast;

namespace TAO3.EventHandlers.Macro;

internal class SendToastNotificationOnMacroCompletion : IDisposable
{
    private readonly IDisposable _disposable;

    public SendToastNotificationOnMacroCompletion(
        IMacroService macroService,
        IToastService toast)
    {
        Dictionary<TAO3Macro, CommandFailed> _commandFailedByMacro = new Dictionary<TAO3Macro, CommandFailed>();

        _disposable = macroService.Events
            .Where(evnt => evnt.Macro.ToastNotificationOnRun)
            .Subscribe(evnt =>
            {
                if (evnt is MacroExecutionFailed failed)
                {
                    _commandFailedByMacro[evnt.Macro] = failed.CommandFailed;
                }

                if (evnt is MacroExecutionCompleted completed)
                {
                    TAO3Macro macro = evnt.Macro;
                    CommandFailed? commandFailed = _commandFailedByMacro.GetValueOrDefault(macro);

                    toast.Notify(
                        FormatToastTitle(macro, commandFailed),
                        FormatToastBody(completed.ExecutionTime, commandFailed),
                        DateTimeOffset.Now + TimeSpan.FromSeconds(1));

                    _commandFailedByMacro.Remove(macro);
                }
            });
    }

    private string FormatToastTitle(TAO3Macro macro, CommandFailed? commandFailed)
    {
        if (commandFailed != null)
        {
            return commandFailed.Message;
        }

        return $"{macro.Name} ran successfully!";
    }

    private string FormatToastBody(TimeSpan executionTime, CommandFailed? commandFailed)
    {
        StringBuilder body = new StringBuilder();
        body.Append("Time elaspsed : ");
        body.Append(executionTime.TotalSeconds.ToString("0.00"));
        body.AppendLine(" secondes");

        if (commandFailed != null)
        {
            body.Append(commandFailed.Message);
            if (commandFailed.Exception != null)
            {
                body.AppendLine();
                body.Append(commandFailed.Exception.ToString());
            }
        }

        return body.ToString();
    }

    public void Dispose()
    {
        _disposable.Dispose();
    }
}
