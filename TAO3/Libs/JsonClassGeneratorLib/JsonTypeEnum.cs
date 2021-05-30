// Copyright © 2010 Xamasoft

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator
{
    internal enum JsonTypeEnum
    {
        Anything = 0,
        String = 1 << 0,
        Boolean = 1 << 1,
        Integer = 1 << 2,
        Long = 1 << 3,
        Float = 1 << 4,
        Date = 1 << 5,
        TimeSpan = 1 << 6,
        Object = 1 << 7,
        Array = 1 << 8,
        Dictionary = 1 << 9,
        NullableSomething = 1 << 10,
        NonConstrained = 1 << 11,

        Nullable = 1 << 12,
        NullableString = Nullable | String,
        NullableInteger = Nullable | Integer,
        NullableLong = Nullable | Long,
        NullableFloat = Nullable | Float,
        NullableBoolean = Nullable | Boolean,
        NullableDate = Nullable | Date,
        NullableTimeSpan = Nullable | TimeSpan,
        NullableObject = Nullable | Object,
        NullableArray = Nullable | Array,
        NullableDictionary = Nullable | Dictionary
    }

    internal static class JsonTypeEnumExtensions
    {
        public static Type ToClrType(this JsonTypeEnum jsonTypeEnum)
        {
            switch (jsonTypeEnum)
            {
                case JsonTypeEnum.Anything:
                case JsonTypeEnum.NonConstrained:
                case JsonTypeEnum.NullableObject:
                case JsonTypeEnum.NullableSomething:
                case JsonTypeEnum.Object:
                    return typeof(object);
                case JsonTypeEnum.Array:
                case JsonTypeEnum.NullableArray: 
                    return typeof(List<>);
                case JsonTypeEnum.Dictionary:
                case JsonTypeEnum.NullableDictionary:
                    return typeof(Dictionary<,>);
                case JsonTypeEnum.Boolean: 
                    return typeof(bool);
                case JsonTypeEnum.Float: 
                    return typeof(double);
                case JsonTypeEnum.Integer:
                    return typeof(int);
                case JsonTypeEnum.Long:
                    return typeof(long);
                case JsonTypeEnum.Date: 
                    return typeof(DateTime);
                case JsonTypeEnum.TimeSpan:
                    return typeof(TimeSpan);
                case JsonTypeEnum.String:
                case JsonTypeEnum.NullableString: 
                    return typeof(string);
                case JsonTypeEnum.NullableBoolean: 
                    return typeof(bool?);
                case JsonTypeEnum.NullableFloat:
                    return typeof(double?);
                case JsonTypeEnum.NullableInteger:
                    return typeof(int?);
                case JsonTypeEnum.NullableLong:
                    return typeof(long?);
                case JsonTypeEnum.NullableTimeSpan:
                    return typeof(TimeSpan?);
                
                default: throw new NotSupportedException("Unsupported json type");
            }
        }
    }
}
