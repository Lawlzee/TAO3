using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class ValueTupleTypeConverter<T1, T2> : TypeConverter<ValueTuple<T1, T2>>
{
    public override bool Convert(StringBuilder sb, ValueTuple<T1, T2> obj, ObjectSerializer serializer, ObjectSerializerOptions options)
    {
        sb.Append("(");
        serializer.Serialize(sb, obj.Item1, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item2, options);
        sb.Append(")");
        return true;
    }
}

internal class ValueTupleTypeConverter<T1, T2, T3> : TypeConverter<ValueTuple<T1, T2, T3>>
{
    public override bool Convert(StringBuilder sb, ValueTuple<T1, T2, T3> obj, ObjectSerializer serializer, ObjectSerializerOptions options)
    {
        sb.Append("(");
        serializer.Serialize(sb, obj.Item1, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item2, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item3, options);
        sb.Append(")");
        return true;
    }
}

internal class ValueTupleTypeConverter<T1, T2, T3, T4> : TypeConverter<ValueTuple<T1, T2, T3, T4>>
{
    public override bool Convert(StringBuilder sb, ValueTuple<T1, T2, T3, T4> obj, ObjectSerializer serializer, ObjectSerializerOptions options)
    {
        sb.Append("(");
        serializer.Serialize(sb, obj.Item1, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item2, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item3, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item4, options);
        sb.Append(")");
        return true;
    }
}

internal class ValueTupleTypeConverter<T1, T2, T3, T4, T5> : TypeConverter<ValueTuple<T1, T2, T3, T4, T5>>
{
    public override bool Convert(StringBuilder sb, ValueTuple<T1, T2, T3, T4, T5> obj, ObjectSerializer serializer, ObjectSerializerOptions options)
    {
        sb.Append("(");
        serializer.Serialize(sb, obj.Item1, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item2, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item3, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item4, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item5, options);
        sb.Append(")");
        return true;
    }
}

internal class ValueTupleTypeConverter<T1, T2, T3, T4, T5, T6> : TypeConverter<ValueTuple<T1, T2, T3, T4, T5, T6>>
{
    public override bool Convert(StringBuilder sb, ValueTuple<T1, T2, T3, T4, T5, T6> obj, ObjectSerializer serializer, ObjectSerializerOptions options)
    {
        sb.Append("(");
        serializer.Serialize(sb, obj.Item1, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item2, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item3, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item4, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item5, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item6, options);
        sb.Append(")");
        return true;
    }
}

internal class ValueTupleTypeConverter<T1, T2, T3, T4, T5, T6, T7> : TypeConverter<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
{
    public override bool Convert(StringBuilder sb, ValueTuple<T1, T2, T3, T4, T5, T6, T7> obj, ObjectSerializer serializer, ObjectSerializerOptions options)
    {
        sb.Append("(");
        serializer.Serialize(sb, obj.Item1, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item2, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item3, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item4, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item5, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item6, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item7, options);
        sb.Append(")");
        return true;
    }
}

internal class ValueTupleTypeConverter<T1, T2, T3, T4, T5, T6, T7, TRest> : TypeConverter<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
     where TRest : struct
{
    public override bool Convert(StringBuilder sb, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> obj, ObjectSerializer serializer, ObjectSerializerOptions options)
    {
        sb.Append("(");
        serializer.Serialize(sb, obj.Item1, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item2, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item3, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item4, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item5, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item6, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Item7, options);
        sb.Append(", ");
        serializer.Serialize(sb, obj.Rest, options);
        sb.Append(")");
        return true;
    }
}
