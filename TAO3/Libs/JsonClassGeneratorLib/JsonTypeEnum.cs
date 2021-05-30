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
}
