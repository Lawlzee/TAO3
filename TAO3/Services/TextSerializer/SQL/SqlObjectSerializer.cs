using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TextSerializer.SQL
{
    public interface ISqlObjectSerializer : IObjectSerializer
    {

    }

    public class SqlObjectSerializer : ObjectSerializer, ISqlObjectSerializer
    {
        public SqlObjectSerializer()
        {
            AddBuiltIn();
        }

        private void AddBuiltIn()
        {
            AddConverter<byte, NumberTypeConverter<byte>>();
            AddConverter<decimal, NumberTypeConverter<decimal>>();
            AddConverter<double, NumberTypeConverter<double>>();
            AddConverter<short, NumberTypeConverter<short>>();
            AddConverter<int, NumberTypeConverter<int>>();
            AddConverter<long, NumberTypeConverter<long>>();
            AddConverter<sbyte, NumberTypeConverter<sbyte>>();
            AddConverter<ushort, NumberTypeConverter<ushort>>();
            AddConverter<uint, NumberTypeConverter<uint>>();
            AddConverter<ulong, NumberTypeConverter<ulong>>();
            AddConverter<float, NumberTypeConverter<float>>();

            AddConverter<bool, BoolTypeConverter>();
            AddConverter<char, CharTypeConverter>();
            AddConverter<string, StringTypeConverter>();
            AddConverter<DateTime, DateTimeTypeConverter>();
            AddConverter<Guid, GuidTypeConverter>();

            AddGenericConverter(typeof(CollectionInitializerTypeConverter<>));

            AddGenericConverter(typeof(NullableTypeConverter<>));

            AddConverter<Enum, EnumTypeConverter>();
            AddConverter<object, ObjectTypeConverter>();
        }
    }
}
