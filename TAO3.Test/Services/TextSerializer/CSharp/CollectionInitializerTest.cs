using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.TextSerializer.CSharp;

namespace ObjectInitializerGenerator.Test
{
    [TestClass]
    public class CollectionInitializerTest
    {
        [TestMethod]
        public void Array1dInitializerTest()
        {
            int[] values = new int[]
            {
                1,
                2,
                3,
                4
            };

            string got = new CSharpObjectSerializer().Serialize(values);

            string expected = @"new int[]
{
    1,
    2,
    3,
    4,
}";

            Assert.AreEqual(expected, got);
        }

        [TestMethod]
        public void ListInitializerTest()
        {
            List<int> values = new List<int>
            {
                1,
                2,
                3,
                4
            };

            string got = new CSharpObjectSerializer().Serialize(values);

            string expected = @"new List<int>
{
    1,
    2,
    3,
    4,
}";

            Assert.AreEqual(expected, got);
        }

        [TestMethod]
        public void DictionaryInitializerTest()
        {
            Dictionary<string, int> value = new Dictionary<string, int>
            {
                ["A"] = 1,
                ["B"] = 2,
                ["C"] = 3
            };

            string got = new CSharpObjectSerializer().Serialize(value);

            string expected = @"new Dictionary<string, int>()
{
    [@""A""] = 1,
    [@""B""] = 2,
    [@""C""] = 3,
}";

            Assert.AreEqual(expected, got);
        }
    }
}
