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
    public class AnonymousTypeTest
    {
        [TestMethod]
        public void AnonymousTypeGenerationTest()
        {
            var obj = new
            {
                A = 5,
                B = true,
                C = 5.5
            };

            string got = new CSharpObjectSerializer().Serialize(obj);
            string expected = @"new
{
    A = 5,
    B = true,
    C = 5.5d
}";

            Assert.AreEqual(expected, got);
        }
    }
}
