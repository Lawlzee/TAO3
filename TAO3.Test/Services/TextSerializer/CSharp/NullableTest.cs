using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.TextSerializer.CSharp;

namespace TAO3.Test.TextSerializer.CSharp
{
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
}
