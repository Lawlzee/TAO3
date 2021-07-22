using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.SQL;

namespace TAO3.Test.Converters.Sql
{
    public record TestObject(
        byte Byte,
        decimal Decimal,
        double Double,
        short Short,
        int Int,
        long Long,
        sbyte SByte,
        ushort UShort,
        uint UInt,
        ulong ULong,
        float Float,
        bool Bool,
        char Char,
        string String,
        DateTime DateTime,
        Guid Guid,
        StringComparison Enum);

    [TestClass]
    public class ObjectTest
    {
        [TestMethod]
        public void SerializeObjectTest()
        {
            List<TestObject> values = new List<TestObject>()
            {
                new TestObject(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, true, 'C', "My String", new DateTime(2021, 01, 02), new Guid("0DFA842C-7F0A-4424-A9AF-881034D03CED"), StringComparison.Ordinal),
                new TestObject(101, 102, 103, 104, 105, 106, 107, 108, 109, 1010, 1011, false, 'D', "My Other String", new DateTime(2021, 01, 02, 03, 04, 05), new Guid("0DFEE61E-DF57-4AFA-B12A-9206232F2D8C"), StringComparison.InvariantCultureIgnoreCase),
            };

            string got = new SqlObjectSerializer().Serialize(values);
            string expected = @"INSERT INTO [TestObject] ([Byte], [Decimal], [Double], [Short], [Int], [Long], [SByte], [UShort], [UInt], [ULong], [Float], [Bool], [Char], [String], [DateTime], [Guid], [Enum]) VALUES(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 1, 'C', 'My String', '2021-01-02', '0DFA842C-7F0A-4424-A9AF-881034D03CED', 4);
INSERT INTO [TestObject] ([Byte], [Decimal], [Double], [Short], [Int], [Long], [SByte], [UShort], [UInt], [ULong], [Float], [Bool], [Char], [String], [DateTime], [Guid], [Enum]) VALUES(101, 102, 103, 104, 105, 106, 107, 108, 109, 1010, 1011, 0, 'D', 'My Other String', '2021-01-02 03:04:05', '0DFEE61E-DF57-4AFA-B12A-9206232F2D8C', 3);";

            Assert.AreEqual(expected, got);
        }

        [TestMethod]
        public void SerializeAnonymousObjectTest()
        {
            var value = new
            {
                A = "A",
                B = 50
            };

            string got = new SqlObjectSerializer().Serialize(value);
            string expected = @"INSERT INTO [<>f__AnonymousType1<string, int>] ([A], [B]) VALUES('A', 50);";

            Assert.AreEqual(expected, got);
        }
    }
}
