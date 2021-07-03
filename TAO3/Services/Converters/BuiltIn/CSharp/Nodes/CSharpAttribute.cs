﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.CSharp
{
    public class CSharpAttribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public AttributeSyntax Syntax { get; }
        public string Name { get; }
        public IReadOnlyList<CSharpAttributeArgument> Arguments { get; }

        public CSharpAttribute(AttributeSyntax syntax)
        {
            Syntax = syntax;
            Name = syntax.Name.ToString();
            Arguments = syntax.ArgumentList == null
                ? new List<CSharpAttributeArgument>()
                : syntax.ArgumentList.Arguments
                    .Select(x => new CSharpAttributeArgument(x))
                    .ToList();
        }

        public static IReadOnlyList<CSharpAttribute> Parse(SyntaxList<AttributeListSyntax> attributeList)
        {
            return attributeList
                .SelectMany(x => x.Attributes)
                .Select(x => new CSharpAttribute(x))
                .ToList();
        }
    }

    public class CSharpAttributeArgument
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public AttributeArgumentSyntax Syntax { get; }
        public string? Name { get; }
        public string Expression { get; }

        public CSharpAttributeArgument(AttributeArgumentSyntax syntax)
        {
            Syntax = syntax;
            Name = syntax.NameColon?.Name?.ToString()
                ?? syntax.NameEquals?.Name?.ToString();
            Expression = syntax.Expression.ToString();
        }
    }
}
