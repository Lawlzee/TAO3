using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsHook;

namespace TAO3.Internal.Interop
{

    internal interface IKeyboardHook
    {
        void RegisterOnKeyPressed(Keys shortcut, Action onPressed);
        bool UnRegisterOnKeyPressed(Keys shortcut);
    }
}
