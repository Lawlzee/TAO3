using System.Runtime.CompilerServices;

namespace TAO3.Internal.Utils;

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
#pragma warning disable RS1024 // Compare symbols correctly
        return RuntimeHelpers.GetHashCode(obj);
#pragma warning restore RS1024 // Compare symbols correctly
    }
}
