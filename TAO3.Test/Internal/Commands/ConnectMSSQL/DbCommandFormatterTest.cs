using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Commands.ConnectMSSQL;

namespace TAO3.Test.Internal.Commands.ConnectMSSQL
{
    [TestClass]
    public class DbCommandFormatterTest
    {
        [TestMethod]
        public void FormatSelectQueryTest()
        {
            string sql = @"SELECT *
FROM MyTable
WHERE Id = @id";

            List<DbParameterMock> parameters = new List<DbParameterMock>
            {
                new DbParameterMock(
                    DbType.Int32,
                    "@id",
                    5)
            };

            DbCommand command = new DbCommandMock(
                CommandType.Text,
                sql,
                parameters);

            string got = new DbCommandFormatter().Format(command);
            string expected = @"SELECT *
FROM MyTable
WHERE Id = 5;";

            Assert.AreEqual(expected, got);
        }

        [TestMethod]
        public void FormatStoredProcedureQueryTest()
        {
            List<DbParameterMock> parameters = new List<DbParameterMock>
            {
                new DbParameterMock(
                    DbType.Int32,
                    "@id",
                    5),
                new DbParameterMock(
                    DbType.Int32,
                    "@id2",
                    10)
            };

            DbCommand command = new DbCommandMock(
                CommandType.StoredProcedure,
                "MyProcedure",
                parameters);

            string got = new DbCommandFormatter().Format(command);
            string expected = @"EXECUTE MyProcedure @id = 5, @id2 = 10;";

            Assert.AreEqual(expected, got);
        }

        [TestMethod]
        public void FormatInsertTest()
        {
            string sql = @"INSERT INTO [MyTable] ([Id], [MyValue]) VALUES(@id, @myValue)";

            List<DbParameterMock> parameters = new List<DbParameterMock>
            {
                new DbParameterMock(
                    DbType.Int32,
                    "@id",
                    5),
                new DbParameterMock(
                    DbType.Int32,
                    "@myValue",
                    10)
            };

            DbCommand command = new DbCommandMock(
                CommandType.Text,
                sql,
                parameters);

            string got = new DbCommandFormatter().Format(command);
            string expected = @"INSERT INTO [MyTable] ([Id], [MyValue])
VALUES               (5, 10);";

            Assert.AreEqual(expected, got);
        }

        [TestMethod]
        public void FormatUpdateTest()
        {
            string sql = @"UPDATE [MyTable] SET Id = @id, MyValue = @myValue WHERE SomeLongColumn <> 1000 AND 1 = 1";

            List<DbParameterMock> parameters = new List<DbParameterMock>
            {
                new DbParameterMock(
                    DbType.Int32,
                    "@id",
                    5),
                new DbParameterMock(
                    DbType.Int32,
                    "@myValue",
                    10)
            };

            DbCommand command = new DbCommandMock(
                CommandType.Text,
                sql,
                parameters);

            string got = new DbCommandFormatter().Format(command);
            string expected = @"UPDATE [MyTable]
SET Id = 5,
    MyValue = 10
WHERE SomeLongColumn <> 1000
      AND 1 = 1;";

            Assert.AreEqual(expected, got);
        }
    }
}
