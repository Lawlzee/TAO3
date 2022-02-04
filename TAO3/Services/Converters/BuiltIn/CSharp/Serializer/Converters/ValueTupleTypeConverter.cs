using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class ValueTupleTypeConverter<T1, T2> : TypeConverter<ValueTuple<T1, T2>, CSharpSerializerSettings>
{
    public override bool Convert(ValueTuple<T1, T2> obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append("(");
        context.Serialize(obj.Item1);
        context.Append(", ");
        context.Serialize(obj.Item2);
        context.Append(")");
        return true;
    }
}

internal class ValueTupleTypeConverter<T1, T2, T3> : TypeConverter<ValueTuple<T1, T2, T3>, CSharpSerializerSettings>
{
    public override bool Convert(ValueTuple<T1, T2, T3> obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append("(");
        context.Serialize(obj.Item1);
        context.Append(", ");
        context.Serialize(obj.Item2);
        context.Append(", ");
        context.Serialize(obj.Item3);
        context.Append(")");
        return true;
    }
}

internal class ValueTupleTypeConverter<T1, T2, T3, T4> : TypeConverter<ValueTuple<T1, T2, T3, T4>, CSharpSerializerSettings>
{
    public override bool Convert(ValueTuple<T1, T2, T3, T4> obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append("(");
        context.Serialize(obj.Item1);
        context.Append(", ");
        context.Serialize(obj.Item2);
        context.Append(", ");
        context.Serialize(obj.Item3);
        context.Append(", ");
        context.Serialize(obj.Item4);
        context.Append(")");
        return true;
    }
}

internal class ValueTupleTypeConverter<T1, T2, T3, T4, T5> : TypeConverter<ValueTuple<T1, T2, T3, T4, T5>, CSharpSerializerSettings>
{
    public override bool Convert(ValueTuple<T1, T2, T3, T4, T5> obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append("(");
        context.Serialize(obj.Item1);
        context.Append(", ");
        context.Serialize(obj.Item2);
        context.Append(", ");
        context.Serialize(obj.Item3);
        context.Append(", ");
        context.Serialize(obj.Item4);
        context.Append(", ");
        context.Serialize(obj.Item5);
        context.Append(")");
        return true;
    }
}

internal class ValueTupleTypeConverter<T1, T2, T3, T4, T5, T6> : TypeConverter<ValueTuple<T1, T2, T3, T4, T5, T6>, CSharpSerializerSettings>
{
    public override bool Convert(ValueTuple<T1, T2, T3, T4, T5, T6> obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append("(");
        context.Serialize(obj.Item1);
        context.Append(", ");
        context.Serialize(obj.Item2);
        context.Append(", ");
        context.Serialize(obj.Item3);
        context.Append(", ");
        context.Serialize(obj.Item4);
        context.Append(", ");
        context.Serialize(obj.Item5);
        context.Append(", ");
        context.Serialize(obj.Item6);
        context.Append(")");
        return true;
    }
}

internal class ValueTupleTypeConverter<T1, T2, T3, T4, T5, T6, T7> : TypeConverter<ValueTuple<T1, T2, T3, T4, T5, T6, T7>, CSharpSerializerSettings>
{
    public override bool Convert(ValueTuple<T1, T2, T3, T4, T5, T6, T7> obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append("(");
        context.Serialize(obj.Item1);
        context.Append(", ");
        context.Serialize(obj.Item2);
        context.Append(", ");
        context.Serialize(obj.Item3);
        context.Append(", ");
        context.Serialize(obj.Item4);
        context.Append(", ");
        context.Serialize(obj.Item5);
        context.Append(", ");
        context.Serialize(obj.Item6);
        context.Append(", ");
        context.Serialize(obj.Item7);
        context.Append(")");
        return true;
    }
}

internal class ValueTupleTypeConverter<T1, T2, T3, T4, T5, T6, T7, TRest> : TypeConverter<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>, CSharpSerializerSettings>
     where TRest : struct
{
    public override bool Convert(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append("(");
        context.Serialize(obj.Item1);
        context.Append(", ");
        context.Serialize(obj.Item2);
        context.Append(", ");
        context.Serialize(obj.Item3);
        context.Append(", ");
        context.Serialize(obj.Item4);
        context.Append(", ");
        context.Serialize(obj.Item5);
        context.Append(", ");
        context.Serialize(obj.Item6);
        context.Append(", ");
        context.Serialize(obj.Item7);
        context.Append(", ");
        context.Serialize(obj.Rest);
        context.Append(")");
        return true;
    }
}
