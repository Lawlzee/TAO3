using System.CommandLine;
using System.Reactive.Disposables;

namespace TAO3.Internal.Extensions;

internal static class IObservableExtensions
{
    public static IDisposable RegisterChildCommand<TEvent, TRegisterEvent, TRemovedEvent>(
        this IObservable<TEvent> observable,
        Command parentCommand,
        Func<TEvent, string> getName,
        Func<TRegisterEvent, Command> createChildCommand)
    {
        return observable.RegisterChildCommand<TEvent, TRegisterEvent, TRemovedEvent>(
            parentCommand,
            getName,
            evnt => (createChildCommand(evnt), Disposable.Empty));
    }

    public static IDisposable RegisterChildCommand<TEvent, TRegisterEvent, TRemovedEvent>(
        this IObservable<TEvent> observable,
        Command parentCommand,
        Func<TEvent, string> getName,
        Func<TRegisterEvent, (Command, IDisposable)> createChildCommand)
    {
        Dictionary<string, IDisposable> removeCommandByName = new();

        return observable.Subscribe(evnt =>
        {
            string name = getName(evnt);

            if (evnt is TRegisterEvent registerEvent)
            {
                (Command childCommand, IDisposable disposable) = createChildCommand(registerEvent);
                removeCommandByName[name] = new CompositeDisposable(
                    disposable,
                    Disposable.Create(() => parentCommand.RemoveSubCommand(childCommand)));

                parentCommand.Add(childCommand);
            }
            else if (evnt is TRemovedEvent)
            {
                if (removeCommandByName.TryGetValue(name, out var unregisterCommand))
                {
                    unregisterCommand.Dispose();
                    removeCommandByName.Remove(name);
                }
            }
        },
        () => 
        { 
            foreach (IDisposable unregisterCommand in removeCommandByName.Values)
            {
                unregisterCommand.Dispose();
            }

            removeCommandByName.Clear();
        });
    }
}
