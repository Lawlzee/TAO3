using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Interop.Windows;

namespace TAO3.Internal.Interop
{
    internal static class InteropFactory
    {
        internal static IInteropOS Create()
        {
            //to do: handle other OS
            return WindowsInterop.Create();
        }
    }
}
