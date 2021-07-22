using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.CSharp;

namespace TAO3.Test.Converters.CSharp
{
    public enum NormalEnum
    {
        Zero = 0,
        One = 1,
        Two = 2
    }

    [Flags]
    public enum FlagEnum
    {
        One = 1,
        Two = 2,
        Four = 4
    }

    [TestClass]
    public class EnumTest
    {
        [TestMethod]
        public void SerializeEnumTest()
        {
            NormalEnum value = NormalEnum.One;
            string got = new CSharpObjectSerializer().Serialize(value);

            string expected = @"NormalEnum.One";

            Assert.AreEqual(expected, got);
        }

        [TestMethod]
        public void SerializeEnumValueNotDefinedTest()
        {
            NormalEnum value = (NormalEnum)100;
            string got = new CSharpObjectSerializer().Serialize(value);

            string expected = @"(NormalEnum)100";

            Assert.AreEqual(expected, got);

        }

        [TestMethod]
        public void SerializeEnumNegativeValueNotDefinedTest()
        {
            NormalEnum value = (NormalEnum)(-100);
            string got = new CSharpObjectSerializer().Serialize(value);

            string expected = @"(NormalEnum)(-100)";

            Assert.AreEqual(expected, got);

        }

        [TestMethod]
        public void SerializeFlagEnumOneValueTest()
        {
            FlagEnum value = FlagEnum.One;
            string got = new CSharpObjectSerializer().Serialize(value);

            string expected = @"FlagEnum.One";

            Assert.AreEqual(expected, got);
        }

        [TestMethod]
        public void SerializeFlagEnumMultipleValuesTest()
        {
            FlagEnum value = FlagEnum.One | FlagEnum.Two;
            string got = new CSharpObjectSerializer().Serialize(value);

            string expected = @"FlagEnum.One | FlagEnum.Two";

            Assert.AreEqual(expected, got);
        }
    }
}
