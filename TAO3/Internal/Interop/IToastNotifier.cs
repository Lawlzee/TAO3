using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Interop
{
    internal interface IToastNotifier
    {
        void Notify(string title, string body, DateTimeOffset expirationTime);
    }
}
