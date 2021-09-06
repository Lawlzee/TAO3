using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Utils
{
    internal class ReferenceEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        private static IEqualityComparer<T>? _defaultComparer;
        public static IEqualityComparer<T> Instance => _defaultComparer ?? (_defaultComparer = new ReferenceEqualityComparer<T>());

        protected ReferenceEqualityComparer() { }

        public bool Equals(T? x, T? y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
