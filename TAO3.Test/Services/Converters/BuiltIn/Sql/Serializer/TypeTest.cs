using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.Sql;

namespace TAO3.Test.Converters.Sql
{
    public record TestTable(
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
    public class TypeTest
    {
        [TestMethod]
        public void SerializeTypeToTableTest()
        {
            string got = new SqlObjectSerializer().Serialize(typeof(TestTable));
            string expected = @"CREATE TABLE [TestTable] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    [Byte] TINYINT NOT NULL,
    [Decimal] DECIMAL(18, 2) NOT NULL,
    [Double] FLOAT NOT NULL,
    [Short] SMALLINT NOT NULL,
    [Int] INT NOT NULL,
    [Long] BIGINT NOT NULL,
    [SByte] SMALLINT NOT NULL,
    [UShort] INT NOT NULL,
    [UInt] BIGINT NOT NULL,
    [ULong] BIGINT NOT NULL,
    [Float] REAL NOT NULL,
    [Bool] BIT NOT NULL,
    [Char] NVARCHAR(1) NOT NULL,
    [String] nvarchar(MAX) NULL,
    [DateTime] DATETIME2 NOT NULL,
    [Guid] UNIQUEIDENTIFIER NOT NULL,
    [Enum] INT NOT NULL);";

            Assert.AreEqual(expected, got);
        }
    }
}
