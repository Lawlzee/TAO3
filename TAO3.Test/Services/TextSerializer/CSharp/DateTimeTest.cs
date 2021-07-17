using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.TextSerializer.CSharp;

namespace TAO3.Test.Services.TextSerializer.CSharp
{
    [TestClass]
    public class DateTimeTest
    {
        [TestMethod]
        public void DateOnlyTest()
        {
            DateTime value = new DateTime(2001, 10, 20);
            string got = new CSharpObjectSerializer().Serialize(value);

            string expected = @"new DateTime(2001, 10, 20)";

            Assert.AreEqual(expected, got);
        }

        [TestMethod]
        public void DateWithTimeTest()
        {
            DateTime value = new DateTime(2001, 10, 20, 05, 15, 25);
            string got = new CSharpObjectSerializer().Serialize(value);

            string expected = @"new DateTime(2001, 10, 20, 05, 15, 25)";

            Assert.AreEqual(expected, got);
        }

        [TestMethod]
        public void DateTimeWithMillisecondTest()
        {
            DateTime value = new DateTime(2001, 10, 20, 05, 15, 25, 250);
            string got = new CSharpObjectSerializer().Serialize(value);

            string expected = @"new DateTime(2001, 10, 20, 05, 15, 25, 250)";

            Assert.AreEqual(expected, got);
        }

        [TestMethod]
        public void DateTimePaddinTest()
        {
            DateTime value = new DateTime(1, 02, 03, 04, 05, 06, 7);
            string got = new CSharpObjectSerializer().Serialize(value);

            string expected = @"new DateTime(1, 02, 03, 04, 05, 06, 7)";

            Assert.AreEqual(expected, got);
        }
    }
}
