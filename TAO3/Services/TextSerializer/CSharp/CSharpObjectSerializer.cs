using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters.CSharp;

namespace TAO3.TextSerializer.CSharp
{
    public interface ICSharpObjectSerializer : IObjectSerializer
    {

    }

    public class CSharpObjectSerializer : ObjectSerializer, ICSharpObjectSerializer
    {
        public CSharpObjectSerializer()
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
            
            AddConverter<ICSharpNode, ICSharpNodeTypeConverter>();
            AddConverter<SyntaxNode, SyntaxNodeTypeConverter>();

            AddGenericConverter(typeof(ValueTupleTypeConverter<,>));
            AddGenericConverter(typeof(ValueTupleTypeConverter<,,>));
            AddGenericConverter(typeof(ValueTupleTypeConverter<,,,>));
            AddGenericConverter(typeof(ValueTupleTypeConverter<,,,,>));
            AddGenericConverter(typeof(ValueTupleTypeConverter<,,,,,>));
            AddGenericConverter(typeof(ValueTupleTypeConverter<,,,,,,>));
            AddGenericConverter(typeof(ValueTupleTypeConverter<,,,,,,,>));

            AddGenericConverter(typeof(CollectionInitializerTypeConverter<>));
            AddGenericConverter(typeof(DictionaryInitializerTypeConverter<,>));
            
            AddGenericConverter(typeof(NullableTypeConverter<>));

            AddConverter<Enum, EnumTypeConverter>();
            AddConverter<object, AnonymousTypeConverter>();
            AddConverter<object, ObjectTypeConverter>();
        }
    }
}
