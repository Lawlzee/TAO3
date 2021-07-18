using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Sql;

namespace TAO3.Test.Services.Sql
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
WHERE Id = 5";

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
            string expected = "EXEC MyProcedure @id = 5, @id2 = 10;";

            Assert.AreEqual(expected, got);
        }
    }
}
