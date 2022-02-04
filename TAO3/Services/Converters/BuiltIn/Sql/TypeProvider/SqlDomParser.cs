using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.IO;
using TAO3.TypeProvider;

namespace TAO3.Converters.Sql;

internal class SqlDomParser : IDomParser<string>
{
    public IDomType Parse(string sql)
    {
        using StringReader stringReader = new StringReader(sql);

        TSql150Parser parser = new TSql150Parser(true, SqlEngineType.All);
        TSqlFragment tree = parser.Parse(stringReader, out IList<ParseError> errors);

        if (errors.Count > 0)
        {
//todo: add custom exception with errors and formatted message
            throw new Exception();
        }

        return Visitor.InferTables(tree);
    }

    private class Visitor : TSqlFragmentVisitor
    {
        private List<IDomType> _tables;

        private string? _tableName;
        private List<List<IDomType>> _rowValues;
        private List<string> _columns;

        private IDomType? _currentType;

        private Visitor()
        {
            _tables = new List<IDomType>();
            _rowValues = new List<List<IDomType>>();
            _columns = new List<string>();
        }

        public static DomCollection InferTables(TSqlFragment fragment)
        {
            Visitor visitor = new Visitor();
            fragment.Accept(visitor);
            return new DomCollection(visitor._tables);
        }

        public override void ExplicitVisit(InsertStatement node)
        {
            _tableName = null;
            _rowValues = new List<List<IDomType>>();
            _columns = new List<string>();
            base.ExplicitVisit(node);

            List<DomClass> inserts = _rowValues
                .Select(values => new DomClass(
                    _tableName!,
                    ZipColumns(_columns, values).ToList()))
                .ToList();

            _tables.AddRange(inserts);
        }

        private IEnumerable<DomClassProperty> ZipColumns(
            List<string> columnNames,
            List<IDomType> values)
        {
            int count = Math.Min(columnNames.Count, values.Count);
            for (int i = 0; i < count; i++)
            {
                yield return new DomClassProperty(
                    columnNames[i],
                    values[i]);
            }

            for (int i = count; i < columnNames.Count; i++)
            {
                yield return new DomClassProperty(
                    columnNames[i],
                    new DomNullLiteral());
            }

            for (int i = count; i < values.Count; i++)
            {
                yield return new DomClassProperty(
                    $"Col{i}",
                    values[i]);
            }
        }

        public override void ExplicitVisit(RowValue node)
        {
            List<IDomType> values = new List<IDomType>();

            foreach (ScalarExpression value in node.ColumnValues)
            {
                _currentType = null;
                value.Accept(this);
                values.Add(_currentType ?? new DomLiteral(typeof(string)));
            }

            _rowValues.Add(values);
        }

        public override void ExplicitVisit(NamedTableReference node)
        {
            _tableName = node.SchemaObject[^1].Value;
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(ColumnReferenceExpression node)
        {
            _columns.Add(node.MultiPartIdentifier[^1].Value);
            base.ExplicitVisit(node);
        }

        #region ScalarExpression
        public override void ExplicitVisit(BinaryLiteral node)
        {
            _currentType = new DomLiteral(typeof(byte[]));
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(NumericLiteral node)
        {
            _currentType = new DomLiteral(typeof(double));
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(RealLiteral node)
        {
            _currentType = new DomLiteral(typeof(double));
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(IntegerLiteral node)
        {
            _currentType = new DomLiteral(typeof(int));
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(MoneyLiteral node)
        {
            _currentType = new DomLiteral(typeof(double));
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(StringLiteral node)
        {
            _currentType = new DomLiteral(typeof(string));
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(NullLiteral node)
        {
            _currentType = new DomNullLiteral();
            base.ExplicitVisit(node);
        }
        #endregion
    }
}
