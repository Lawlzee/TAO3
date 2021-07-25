using System;

namespace TAO3.TypeProvider
{
    public interface ILiteralTypeMerger
    {
        static ILiteralTypeMerger Default { get; } = new LiteralTypeMerger();
        Type Merge(Type typeA, Type typeB);
    }
}
