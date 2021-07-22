using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.CSharp;

namespace TAO3.Test.Converters.CSharp
{
    [TestClass]
    public class TupleTest
    {
        [TestMethod]
        public void TupleGenerationTest()
        {
            (int, string, Guid, DateTime, float, (double, bool)) value = (5, @"test string", new Guid("38c8fabd-4376-4620-9518-9ec1725e13dd"), new DateTime(2020, 1, 12, 10, 23, 32, 0), 12.23f, (20.345, false));

            string got = new CSharpObjectSerializer().Serialize(value);
            string expected = @"(5, @""test string"", new Guid(""38c8fabd-4376-4620-9518-9ec1725e13dd""), new DateTime(2020, 01, 12, 10, 23, 32), 12.23f, (20.345d, false))";

            Assert.AreEqual(expected, got);
        }
    }
}
