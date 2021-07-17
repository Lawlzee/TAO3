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
    public class ObjectTest
    {
        [TestMethod]
        public void RecordInitializerTest()
        {
            TestRecord value = new TestRecord(5, "abcd", 82);

            string got = new CSharpObjectSerializer().Serialize(value);

            string expected = @"new ObjectTest.TestRecord(
    A: 5,
    B: @""abcd"",
    C: 82UL)";

            Assert.AreEqual(expected, got);
        }

        private record TestRecord(int A, string B, ulong C);

        [TestMethod]
        public void DTOStyleObjectInitializerTest()
        {
            DTO value = new DTO
            {
                A = 12,
                B = 'B',
                C = new DateTime(2020, 10, 12),
            };

            string got = new CSharpObjectSerializer().Serialize(value);

            string expected = @"new ObjectTest.DTO()
{
    A = 12,
    B = 'B',
    C = new DateTime(2020, 10, 12),
}";

            Assert.AreEqual(expected, got);
        }

        private class DTO
        {
            public int A { get; set; }
            public char B { get; set; }
            public DateTime C { get; set; }
        }
    }
}
