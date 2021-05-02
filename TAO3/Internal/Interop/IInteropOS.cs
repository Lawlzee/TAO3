using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Interop
{
    internal interface IInteropOS
    {
        IKeyboardHook KeyboardHook { get; }
        IClipboard Clipboard { get; }
    }
}
