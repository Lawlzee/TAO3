using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Formatting
{
    public class SqlFormatter : IFormatter
    {
        public string Format(string sql)
        {
            using StringReader stringReader = new StringReader(sql);
            TSql150Parser parser = new TSql150Parser(true, SqlEngineType.All);
            TSqlFragment tree = parser.Parse(stringReader, out IList<ParseError> errors);

            if (errors.Count > 0)
            {
                //todo: add custom exception with errors and formatted message
                throw new Exception();
            }

            Sql150ScriptGenerator scriptGenerator = new Sql150ScriptGenerator(new SqlScriptGeneratorOptions
            {
                AlignClauseBodies = false,
                AlignSetClauseItem = false,

            });
            scriptGenerator.GenerateScript(tree, out string script);

            return script.Trim();
        }
    }
}
