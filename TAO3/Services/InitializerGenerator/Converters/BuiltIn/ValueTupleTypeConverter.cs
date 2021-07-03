using System;
using System.Collections.Generic;
using System.Text;

namespace TAO3.InitializerGenerator.Converters
{
    internal class ValueTupleTypeConverter<T1, T2> : TypeConverter<ValueTuple<T1, T2>>
    {
        public override bool Convert(StringBuilder sb, ValueTuple<T1, T2> obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append("(");
            generator.Generate(sb, obj.Item1, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item2, options);
            sb.Append(")");
            return true;
        }
    }

    internal class ValueTupleTypeConverter<T1, T2, T3> : TypeConverter<ValueTuple<T1, T2, T3>>
    {
        public override bool Convert(StringBuilder sb, ValueTuple<T1, T2, T3> obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append("(");
            generator.Generate(sb, obj.Item1, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item2, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item3, options);
            sb.Append(")");
            return true;
        }
    }

    internal class ValueTupleTypeConverter<T1, T2, T3, T4> : TypeConverter<ValueTuple<T1, T2, T3, T4>>
    {
        public override bool Convert(StringBuilder sb, ValueTuple<T1, T2, T3, T4> obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append("(");
            generator.Generate(sb, obj.Item1, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item2, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item3, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item4, options);
            sb.Append(")");
            return true;
        }
    }

    internal class ValueTupleTypeConverter<T1, T2, T3, T4, T5> : TypeConverter<ValueTuple<T1, T2, T3, T4, T5>>
    {
        public override bool Convert(StringBuilder sb, ValueTuple<T1, T2, T3, T4, T5> obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append("(");
            generator.Generate(sb, obj.Item1, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item2, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item3, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item4, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item5, options);
            sb.Append(")");
            return true;
        }
    }

    internal class ValueTupleTypeConverter<T1, T2, T3, T4, T5, T6> : TypeConverter<ValueTuple<T1, T2, T3, T4, T5, T6>>
    {
        public override bool Convert(StringBuilder sb, ValueTuple<T1, T2, T3, T4, T5, T6> obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append("(");
            generator.Generate(sb, obj.Item1, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item2, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item3, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item4, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item5, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item6, options);
            sb.Append(")");
            return true;
        }
    }

    internal class ValueTupleTypeConverter<T1, T2, T3, T4, T5, T6, T7> : TypeConverter<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        public override bool Convert(StringBuilder sb, ValueTuple<T1, T2, T3, T4, T5, T6, T7> obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append("(");
            generator.Generate(sb, obj.Item1, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item2, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item3, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item4, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item5, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item6, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item7, options);
            sb.Append(")");
            return true;
        }
    }

    internal class ValueTupleTypeConverter<T1, T2, T3, T4, T5, T6, T7, TRest> : TypeConverter<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
         where TRest : struct
    {
        public override bool Convert(StringBuilder sb, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> obj, InitializerGeneratorService generator, InitializerGeneratorOptions options)
        {
            sb.Append("(");
            generator.Generate(sb, obj.Item1, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item2, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item3, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item4, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item5, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item6, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Item7, options);
            sb.Append(", ");
            generator.Generate(sb, obj.Rest, options);
            sb.Append(")");
            return true;
        }
    }
}
