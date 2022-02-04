using Microsoft.VisualStudio.TestTools.UnitTesting;
using TAO3.Converters.CSharp;

namespace TAO3.Test.Converters.CSharp;

[TestClass]
public class NullableTest
{
    [TestMethod]
    public void SerializeNullTest()
    {
        int? value = null;
        string got = new CSharpObjectSerializer().Serialize(value);

        string expected = @"null";

        Assert.AreEqual(expected, got);
    }

    [TestMethod]
    public void SerializeNullableValueTest()
    {
        int? value = 6;
        string got = new CSharpObjectSerializer().Serialize(value);

        string expected = @"6";

        Assert.AreEqual(expected, got);
    }
}
