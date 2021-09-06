using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Avalonia;
using TAO3.Macro;

namespace TAO3.EventHandlers.Macro
{
    internal class ShowMacrosInAvalonia : IDisposable
    {
        private IDisposable? _subscription;

        private ShowMacrosInAvalonia() { }

        public static async Task<IDisposable> CreateAsync(IAvaloniaService avaloniaService, IMacroService macroService)
        {
            ShowMacrosInAvalonia instance = new ShowMacrosInAvalonia();

            avaloniaService.ApplicationStoped.Subscribe(_ =>
            {
                instance._subscription?.Dispose();
                instance._subscription = null;
            });

            Dictionary<TAO3Macro, TabItem> _tabByMacro = new Dictionary<TAO3Macro, TabItem>();

            avaloniaService.ApplicationStarted
                .SelectMany(async _ =>
                {
                    await OnApplicationStartedAsync();
                    return Unit.Default;
                })
                .Subscribe();

            if (avaloniaService.IsApplicationStarted)
            {
                await OnApplicationStartedAsync();
            }

            return instance;

            async Task OnApplicationStartedAsync()
            {
                foreach (TAO3Macro macro in macroService.Macros)
                {
                    TabItem tabItem = await OnMacroAddedAsync(macro);
                    _tabByMacro[macro] = tabItem;
                }

                instance._subscription = macroService.Events
                    .SelectMany(async evnt =>
                    {
                        TAO3Macro macro = evnt.Macro;

                        if (evnt is MacroAdded)
                        {
                            TabItem tabItem = await OnMacroAddedAsync(macro);
                            _tabByMacro[macro] = tabItem;
                        }

                        if (evnt is MacroRemoved)
                        {
                            await avaloniaService.RemoveTabAsync(_tabByMacro[macro]);
                            _tabByMacro.Remove(macro);
                        }

                        return Unit.Default;
                    })
                    .Subscribe();
            }

            Task<TabItem> OnMacroAddedAsync(TAO3Macro macro)
            {
                return avaloniaService.AddTabAsync(() => CreateTab(macro));
            }

            TabItem CreateTab(TAO3Macro macro)
            {
                return new TabItem
                {
                    Header = macro.Name,
                    FontSize = 14,
                    Content = new TextBox
                    {
                        Text = macro.Code
                    }
                };
            }
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
