using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;

namespace TAO3.Internal.Commands.ConnectMSSQL;

internal class DbCommandFormatter
{
    public string Format(DbCommand command)
    {
        using StringReader stringReader = new StringReader(command.CommandText);

        TSql150Parser parser = new TSql150Parser(true, SqlEngineType.All);
        TSqlFragment tree = parser.Parse(stringReader, out IList<ParseError> errors);

        if (errors.Count > 0)
        {
            //todo: add custom exception with errors and formatted message
            throw new Exception();
        }

        Dictionary<string, string> parameterValueByName = command.Parameters
            .Cast<DbParameter>()
            .ToDictionary(
                x => x.ParameterName,
                x => GetParameterValue(x));

        if (command.CommandType == CommandType.StoredProcedure)
        {
            return IndentQuery(FormatStoredProcedure(command.CommandText, parameterValueByName));
        }

        return IndentQuery(FormatQuery(tree, parameterValueByName));
    }

    private string GetParameterValue(DbParameter sqlParameter)
    {
        if (sqlParameter.Value == DBNull.Value || sqlParameter.Value == null)
        {
            return "null";
        }

        switch (sqlParameter.DbType)
        {
            case DbType.String:
            case DbType.StringFixedLength:
                return $"N'{sqlParameter.Value.ToString()!.Replace("'", "''")}'";
            case DbType.AnsiString:
            case DbType.AnsiStringFixedLength:
            case DbType.Xml:
            case DbType.Time:
            case DbType.Guid:
                return $"'{sqlParameter.Value.ToString()!.Replace("'", "''")}'";
            case DbType.Date:
            case DbType.DateTime:
            case DbType.DateTime2:
            case DbType.DateTimeOffset:
                return ((DateTime)sqlParameter.Value).ToString("yyyy-MM-dd HH:mm:ss:fff");
            case DbType.Boolean:
                return (bool)sqlParameter.Value ? "1" : "0";
            case DbType.Decimal:
                return ((decimal)sqlParameter.Value).ToString(CultureInfo.InvariantCulture);
            case DbType.Single:
                return ((float)sqlParameter.Value).ToString(CultureInfo.InvariantCulture);
            case DbType.Double:
                return ((double)sqlParameter.Value).ToString(CultureInfo.InvariantCulture);
            default:
                return sqlParameter.Value.ToString()?.Replace("'", "''") ?? "null";
        }
    }


    private string FormatStoredProcedure(
        string storedProcedureName,
        Dictionary<string, string> parameterValueByName)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("EXEC ");
        sb.Append(storedProcedureName);

        bool isFirst = true;
        foreach ((string name, string value) in parameterValueByName)
        {
            if (!isFirst)
            {
                sb.Append(",");
            }

            sb.Append(" ");
            sb.Append(name);
            sb.Append(" = ");
            sb.Append(value);

            isFirst = false;
        }

        sb.Append(";");

        return sb.ToString();
    }

    private string FormatQuery(
        TSqlFragment tree,
        Dictionary<string, string> parameterValueByName)
    {
        StringBuilder result = new StringBuilder();
        foreach (TSqlParserToken token in tree.ScriptTokenStream)
        {
            if (token.TokenType == TSqlTokenType.Variable
                && parameterValueByName.TryGetValue(token.Text, out string? value))
            {
                result.Append(value!);
            }
            else
            {
                result.Append(token.Text);
            }
        }

        return result.ToString();
    }

    private string IndentQuery(string query)
    {
        using StringReader stringReader = new StringReader(query);

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
