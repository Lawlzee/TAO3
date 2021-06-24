using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Extensions
{
    internal static class SymbolSetExtensions
    {
        private static readonly Action<SymbolSet, ISymbol> _remove = typeof(SymbolSet)
            .GetMethod(
                name: "Remove", 
                BindingFlags.NonPublic | BindingFlags.Instance, 
                binder: null, 
                new Type[] { typeof(ISymbol) }, 
                modifiers: null)
            !.CreateDelegate<Action<SymbolSet, ISymbol>>();

        public static void Remove(this SymbolSet symbolSet, ISymbol symbol) => _remove(symbolSet, symbol);
    }
}
