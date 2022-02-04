using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TAO3.TypeProvider;

namespace TAO3.Test.TypeProvider;

public static class TypeProviderAsserts
{
    public static void AssertEquals(IDomType expected, IDomType got)
    {
        Assert.AreEqual(expected.GetType(), got.GetType());
        if (expected is DomClass)
        {
            AssertEquals((DomClass)expected, (DomClass)got);
            return;
        }

        if (expected is DomCollection)
        {
            AssertEquals((DomCollection)expected, (DomCollection)got);
            return;
        }

        if (expected is DomClass)
        {
            AssertEquals((DomClass)expected, (DomClass)got);
            return;
        }

        if (expected is DomLiteral)
        {
            AssertEquals((DomLiteral)expected, (DomLiteral)got);
            return;
        }

        if (expected is DomNullLiteral)
        {
            return;
        }

        throw new ArgumentException($"{expected.GetType().FullName} not supported");
    }

    public static void AssertEquals(DomClass expected, DomClass got)
    {
        Assert.AreEqual(expected.Name, got.Name);

        Assert.AreEqual(expected.Properties.Count, got.Properties.Count);
        for (int i = 0; i < expected.Properties.Count; i++)
        {
            AssertEquals(expected.Properties[i], got.Properties[i]);
        }
    }

    public static void AssertEquals(DomClassProperty expected, DomClassProperty got)
    {
        Assert.AreEqual(expected.Name, got.Name);
        AssertEquals(expected.Type, got.Type);
    }

    public static void AssertEquals(DomLiteral expected, DomLiteral got)
    {
        Assert.AreEqual(expected.Type, got.Type);
    }

    public static void AssertEquals(DomCollection expected, DomCollection got)
    {
        Assert.AreEqual(expected.Values.Count, got.Values.Count);
        for (int i = 0; i < expected.Values.Count; i++)
        {
            AssertEquals(expected.Values[i], got.Values[i]);
        }
    }
}
