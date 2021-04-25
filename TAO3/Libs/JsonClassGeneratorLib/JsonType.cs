// Copyright © 2010 Xamasoft

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Xamasoft.JsonClassGenerator
{
    internal class JsonType
    {
        internal JsonTypeEnum Type { get; private set; }
        internal JsonType InternalType { get; private set; }
        internal string AssignedName { get; private set; }
        internal IList<FieldInfo> Fields { get; set; }
        internal bool IsRoot { get; set; }

        internal JsonType(JToken token)
        {

            Type = GetFirstTypeEnum(token);

            if (Type == JsonTypeEnum.Array)
            {
                JArray? array = (JArray)token;
                InternalType = GetCommonType(array.ToArray());
            }
        }

        internal JsonType(JsonTypeEnum type)
        {
            Type = type;
        }

        internal static JsonType GetNull()
        {
            return new JsonType(JsonTypeEnum.NullableSomething);
        }

        internal static JsonType GetCommonType(JToken[] tokens)
        {

            if (tokens.Length == 0)
            {
                return new JsonType(JsonTypeEnum.NonConstrained);
            }

            JsonType? common = new JsonType(tokens[0]);

            for (int i = 1; i < tokens.Length; i++)
            {
                JsonType? current = new JsonType(tokens[i]);
                common = common.GetCommonType(current);
            }

            return common;

        }

        internal void AssignName(string name)
        {
            AssignedName = name;
        }

        internal JsonType GetInnermostType()
        {
            if (Type != JsonTypeEnum.Array)
            {
                throw new InvalidOperationException();
            }

            if (InternalType.Type != JsonTypeEnum.Array)
            {
                return InternalType;
            }

            return InternalType.GetInnermostType();
        }

        internal JsonType GetCommonType(JsonType type2)
        {
            JsonTypeEnum commonType = GetCommonTypeEnum(Type, type2.Type);

            if (commonType == JsonTypeEnum.Array)
            {
                if (type2.Type == JsonTypeEnum.NullableSomething)
                {
                    return this;
                }

                if (Type == JsonTypeEnum.NullableSomething)
                {
                    return type2;
                }

                JsonType? commonInternalType = InternalType.GetCommonType(type2.InternalType);
                if (commonInternalType != InternalType)
                {
                    return new JsonType(JsonTypeEnum.Array) 
                    { 
                        InternalType = commonInternalType 
                    };
                }
            }

            if (Type == commonType)
            {
                return this;
            }

            return new JsonType(commonType);
        }


        private static bool IsNull(JsonTypeEnum type)
        {
            return type == JsonTypeEnum.NullableSomething;
        }

        private JsonTypeEnum GetCommonTypeEnum(JsonTypeEnum type1, JsonTypeEnum type2)
        {
            if (type1 == JsonTypeEnum.NonConstrained)
            {
                return type2;
            }

            if (type2 == JsonTypeEnum.NonConstrained)
            {
                return type1;
            }

            switch (type1)
            {
                case JsonTypeEnum.Boolean:
                    if (IsNull(type2))
                    {
                        return JsonTypeEnum.NullableBoolean;
                    }

                    if (type2 == JsonTypeEnum.Boolean)
                    {
                        return type1;
                    }

                    break;
                case JsonTypeEnum.NullableBoolean:
                    if (IsNull(type2))
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.Boolean)
                    {
                        return type1;
                    }

                    break;
                case JsonTypeEnum.Integer:
                    if (IsNull(type2))
                    {
                        return JsonTypeEnum.NullableInteger;
                    }

                    if (type2 == JsonTypeEnum.Float)
                    {
                        return JsonTypeEnum.Float;
                    }

                    if (type2 == JsonTypeEnum.Long)
                    {
                        return JsonTypeEnum.Long;
                    }

                    if (type2 == JsonTypeEnum.Integer)
                    {
                        return type1;
                    }

                    break;
                case JsonTypeEnum.NullableInteger:
                    if (IsNull(type2))
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.Float)
                    {
                        return JsonTypeEnum.NullableFloat;
                    }

                    if (type2 == JsonTypeEnum.Long)
                    {
                        return JsonTypeEnum.NullableLong;
                    }

                    if (type2 == JsonTypeEnum.Integer)
                    {
                        return type1;
                    }

                    break;
                case JsonTypeEnum.Float:
                    if (IsNull(type2))
                    {
                        return JsonTypeEnum.NullableFloat;
                    }

                    if (type2 == JsonTypeEnum.Float)
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.Integer)
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.Long)
                    {
                        return type1;
                    }

                    break;
                case JsonTypeEnum.NullableFloat:
                    if (IsNull(type2))
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.Float)
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.Integer)
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.Long)
                    {
                        return type1;
                    }

                    break;
                case JsonTypeEnum.Long:
                    if (IsNull(type2))
                    {
                        return JsonTypeEnum.NullableLong;
                    }

                    if (type2 == JsonTypeEnum.Float)
                    {
                        return JsonTypeEnum.Float;
                    }

                    if (type2 == JsonTypeEnum.Integer)
                    {
                        return type1;
                    }

                    break;
                case JsonTypeEnum.NullableLong:
                    if (IsNull(type2))
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.Float)
                    {
                        return JsonTypeEnum.NullableFloat;
                    }

                    if (type2 == JsonTypeEnum.Integer)
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.Long)
                    {
                        return type1;
                    }

                    break;
                case JsonTypeEnum.Date:
                    if (IsNull(type2))
                    {
                        return JsonTypeEnum.NullableDate;
                    }

                    if (type2 == JsonTypeEnum.Date)
                    {
                        return JsonTypeEnum.Date;
                    }

                    break;
                case JsonTypeEnum.NullableDate:
                    if (IsNull(type2))
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.Date)
                    {
                        return type1;
                    }

                    break;
                case JsonTypeEnum.NullableSomething:
                    if (IsNull(type2))
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.String)
                    {
                        return JsonTypeEnum.String;
                    }

                    if (type2 == JsonTypeEnum.Integer)
                    {
                        return JsonTypeEnum.NullableInteger;
                    }

                    if (type2 == JsonTypeEnum.Float)
                    {
                        return JsonTypeEnum.NullableFloat;
                    }

                    if (type2 == JsonTypeEnum.Long)
                    {
                        return JsonTypeEnum.NullableLong;
                    }

                    if (type2 == JsonTypeEnum.Boolean)
                    {
                        return JsonTypeEnum.NullableBoolean;
                    }

                    if (type2 == JsonTypeEnum.Date)
                    {
                        return JsonTypeEnum.NullableDate;
                    }

                    if (type2 == JsonTypeEnum.Array)
                    {
                        return JsonTypeEnum.Array;
                    }

                    if (type2 == JsonTypeEnum.Object)
                    {
                        return JsonTypeEnum.Object;
                    }

                    break;
                case JsonTypeEnum.Object:
                    if (IsNull(type2))
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.Object)
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.Dictionary)
                    {
                        throw new ArgumentException();
                    }

                    break;
                case JsonTypeEnum.Dictionary:
                    throw new ArgumentException();
                //if (IsNull(type2)) return type1;
                //if (type2 == JsonTypeEnum.Object) return type1;
                //if (type2 == JsonTypeEnum.Dictionary) return type1;
                //  break;
                case JsonTypeEnum.Array:
                    if (IsNull(type2))
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.Array)
                    {
                        return type1;
                    }

                    break;
                case JsonTypeEnum.String:
                    if (IsNull(type2))
                    {
                        return type1;
                    }

                    if (type2 == JsonTypeEnum.String)
                    {
                        return type1;
                    }

                    break;
            }

            return JsonTypeEnum.Anything;

        }

        private static JsonTypeEnum GetFirstTypeEnum(JToken token)
        {
            JTokenType type = token.Type;
            if (type == JTokenType.Integer)
            {
                if ((long)((JValue)token).Value < int.MaxValue)
                {
                    return JsonTypeEnum.Integer;
                }
                else
                {
                    return JsonTypeEnum.Long;
                }
            }
            switch (type)
            {
                case JTokenType.Array: return JsonTypeEnum.Array;
                case JTokenType.Boolean: return JsonTypeEnum.Boolean;
                case JTokenType.Float: return JsonTypeEnum.Float;
                case JTokenType.Null: return JsonTypeEnum.NullableSomething;
                case JTokenType.Undefined: return JsonTypeEnum.NullableSomething;
                case JTokenType.String: return JsonTypeEnum.String;
                case JTokenType.Object: return JsonTypeEnum.Object;
                case JTokenType.Date: return JsonTypeEnum.Date;

                default: return JsonTypeEnum.Anything;

            }
        }
    }
}
