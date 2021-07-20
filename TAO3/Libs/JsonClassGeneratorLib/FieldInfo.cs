// Copyright © 2010 Xamasoft

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAO3.Internal.CodeGeneration;

namespace Xamasoft.JsonClassGenerator
{
    internal class FieldInfo
    {
        internal string MemberName { get; private set; }
        internal string JsonMemberName { get; private set; }
        internal JsonType Type { get; private set; }

        internal FieldInfo(string jsonMemberName, JsonType type)
        {
            JsonMemberName = jsonMemberName;
            MemberName = IdentifierUtils.ToCSharpIdentifier(jsonMemberName);
            Type = type;
        }
    }
}
