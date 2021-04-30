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
        Object = 1 << 6,
        Array = 1 << 7,
        Dictionary = 1 << 8,
        NullableSomething = 1 << 9,
        NonConstrained = 1 << 10,

        Nullable = 1 << 11,
        NullableInteger = Nullable | Integer,
        NullableLong = Nullable | Long,
        NullableFloat = Nullable | Float,
        NullableBoolean = Nullable | Boolean,
        NullableDate = Nullable | Date
    }
}
