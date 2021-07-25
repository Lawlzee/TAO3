using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.Sql;
using TAO3.Test.TypeProvider;
using TAO3.TypeProvider;

namespace TAO3.Test.Converters.Sql
{
    [TestClass]
    public class SqlTDomParserTest
    {
        [TestMethod]
        public void DeserializeInsertStatementTest()
        {
            string sql = @"INSERT INTO MyTable (Col1, Col2) VALUES('A', 2);";
            IDomType got = new SqlDomParser().Parse(sql);
            DomCollection expected = new DomCollection(
                new List<IDomType>
                {
                    new DomClass(
                        "MyTable",
                        new List<DomClassProperty>
                        {
                            new DomClassProperty(
                                "Col1",
                                new DomLiteral(
                                    typeof(string))),
                            new DomClassProperty(
                                "Col2",
                                new DomLiteral(
                                    typeof(int)))
                        })
                });

            TypeProviderAsserts.AssertEquals(expected, got);
        }
    }
}
