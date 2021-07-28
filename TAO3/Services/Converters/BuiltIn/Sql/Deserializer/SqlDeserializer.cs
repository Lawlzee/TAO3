using Microsoft.SqlServer.TransactSql.ScriptDom;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.Sql
{
    public class SqlDeserializer
    {
        private readonly Dictionary<Type, SqlSchemaBinder> _binderCacheByType;

        public SqlDeserializer()
        {
            _binderCacheByType = new Dictionary<Type, SqlSchemaBinder>();
        }

        public List<T> Deserialize<T>(string sql)
            where T : new()
        {
            if (!_binderCacheByType.ContainsKey(typeof(T)))
            {
                _binderCacheByType[typeof(T)] = SqlSchemaBinder.Create<T>();
            }

            using StringReader stringReader = new StringReader(sql);

            TSql150Parser parser = new TSql150Parser(true, SqlEngineType.All);
            TSqlFragment tree = parser.Parse(stringReader, out IList<ParseError> errors);

            if (errors.Count > 0)
            {
                //todo: add custom exception with errors and formatted message
                throw new Exception();
            }

            return Visitor<T>.Parse(_binderCacheByType[typeof(T)], tree);
        }


        private class Visitor<T> : TSqlFragmentVisitor
            where T : new()
        {
            private readonly SqlSchemaBinder _binder;
            private readonly List<T> _rows;

            private List<List<string?>> _rowValues;
            private string? _value;
            private bool _isNull;

            private List<string> _columns;

            public Visitor(SqlSchemaBinder binder)
            {
                _binder = binder;
                _rows = new List<T>();
                _rowValues = new List<List<string?>>();
                _columns = new List<string>();
            }

            public static List<T> Parse(SqlSchemaBinder binder, TSqlFragment fragment)
            {
                Visitor<T> visitor = new Visitor<T>(binder);
                fragment.Accept(visitor);
                return visitor._rows;
            }

            public override void ExplicitVisit(InsertStatement node)
            {
                base.ExplicitVisit(node);

                _rows.AddRange(_rowValues.Select(rowValue => CreateRow(_columns, rowValue)));

                _rowValues.Clear();
                _columns.Clear();
            }

            private T CreateRow(List<string> columns, List<string?> values)
            {
                T row = new T();

                int count = Math.Min(columns.Count, values.Count);
                for (int i = 0; i < count; i++)
                {
                    string column = columns[i];
                    string? value = values[i];

                    Type type = _binder.GetType(column);
                    object? convertedValue = Convert.ChangeType(value, type);

                    _binder.SetValue(row, column, convertedValue);
                }

                for (int i = count; i < values.Count; i++)
                {
                    string? value = values[i];

                    Type type = _binder.GetType(i);
                    object? convertedValue = Convert.ChangeType(value, type);

                    _binder.SetValue(row, i, convertedValue);
                }

                return row;
            }

            public override void ExplicitVisit(RowValue node)
            {
                List<string?> values = new List<string?>();

                foreach (ScalarExpression value in node.ColumnValues)
                {
                    _value = null;
                    _isNull = false;
                    value.Accept(this);
                    if (!_isNull && _value == null)
                    {
                        throw new Exception($"Insert value of type '{value.GetType().FullName}' not supporter");
                    }
                    values.Add(_value);
                }

                _rowValues.Add(values);
            }

            public override void ExplicitVisit(ColumnReferenceExpression node)
            {
                _columns.Add(node.MultiPartIdentifier[^1].Value);
                base.ExplicitVisit(node);
            }

            #region ScalarExpression
            public override void ExplicitVisit(BinaryLiteral node)
            {
                _value = node.Value;
                base.ExplicitVisit(node);
            }

            public override void ExplicitVisit(NumericLiteral node)
            {
                _value = node.Value;
                base.ExplicitVisit(node);
            }

            public override void ExplicitVisit(RealLiteral node)
            {
                _value = node.Value;
                base.ExplicitVisit(node);
            }

            public override void ExplicitVisit(IntegerLiteral node)
            {
                _value = node.Value;
                base.ExplicitVisit(node);
            }

            public override void ExplicitVisit(MoneyLiteral node)
            {
                _value = node.Value;
                base.ExplicitVisit(node);
            }

            public override void ExplicitVisit(StringLiteral node)
            {
                _value = node.Value;
                base.ExplicitVisit(node);
            }

            public override void ExplicitVisit(NullLiteral node)
            {
                _isNull = true;
                base.ExplicitVisit(node);
            }
            #endregion
        }
    }
}
