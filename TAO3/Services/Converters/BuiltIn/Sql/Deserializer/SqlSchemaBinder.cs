using CsvHelper.Configuration.Attributes;
using FastExpressionCompiler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.Sql
{
    internal class SqlSchemaBinder
    {
        private record SchemaProperty(Type Type, Action<object, object?> Setter);

        private readonly Dictionary<string, SchemaProperty> _propertyByName;
        private readonly Dictionary<int, SchemaProperty> _propertByIndex;

        private SqlSchemaBinder(
            Dictionary<string, SchemaProperty> propertyByName, 
            Dictionary<int, SchemaProperty> propertByIndex)
        {
            _propertyByName = propertyByName;
            _propertByIndex = propertByIndex;
        }

        public static SqlSchemaBinder Create<T>()
        {
            var props = typeof(T).GetProperties()
                .Select(x => new
                {
                    Property = x,
                    Name = x.GetCustomAttributes(typeof(JsonPropertyAttribute), true)
                        .OfType<JsonPropertyAttribute>()
                        .FirstOrDefault()?.PropertyName
                        ?? x.Name,
                    Index = x.GetCustomAttributes(typeof(IndexAttribute), true)
                        .OfType<IndexAttribute>()
                        .FirstOrDefault()?.Index,
                    Schema = new SchemaProperty(
                        x.PropertyType,
                        CreateSetProperty(x))
                })
                .ToList();

            Dictionary<string, SchemaProperty> setByColumnName = props
                .GroupBy(x => x.Name)
                .Select(x => x.First())
                .ToDictionary(
                    x => x.Name,
                    x => x.Schema);

            HashSet<int> indexesUsed = props
                .Select(x => x.Index)
                .Where(x => x != null)
                .Select(x => x!.Value)
                .ToHashSet();

            int currentIndex = 0;
            var propsWithIndex = props
                .Select(x => new
                {
                    x.Property,
                    Index = x.Index ?? GetNextIndex(),
                    x.Schema
                })
                .ToList();

            Dictionary<int, SchemaProperty> setByIndex = propsWithIndex
                .GroupBy(x => x.Index)
                .Select(x => x.First())
                .ToDictionary(
                    x => x.Index,
                    x => x.Schema);

            return new SqlSchemaBinder(setByColumnName, setByIndex);

            Action<object, object?> CreateSetProperty(PropertyInfo propertyInfo)
            {
                ParameterExpression objParam = Expression.Parameter(typeof(object));
                ParameterExpression valueParam = Expression.Parameter(typeof(object));

                return Expression
                    .Lambda<Action<object, object?>>(
                        Expression.Assign(
                            Expression.MakeMemberAccess(
                                Expression.Convert(
                                    objParam,
                                    typeof(T)),
                                propertyInfo),
                            Expression.Convert(
                                valueParam,
                                propertyInfo.PropertyType)),
                        objParam,
                        valueParam)
                    .CompileFast();
            }

            int GetNextIndex()
            {
                while (indexesUsed.Contains(currentIndex))
                {
                    currentIndex++;
                }

                indexesUsed.Add(currentIndex);
                return currentIndex;
            }
        }

        public Type GetType(int index)
        {
            return _propertByIndex[index].Type;
        }

        public Type GetType(string columnName)
        {
            return _propertyByName[columnName].Type;
        }

        public void SetValue<T>(T obj, int index, object? value)
        {
            _propertByIndex[index].Setter(obj!, value);
        }

        public void SetValue<T>(T obj, string columnName, object? value)
        {
            _propertyByName[columnName].Setter(obj!, value);
        }
    }
}
