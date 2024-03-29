﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace TAO3.Converters.CSharp;

public class CSharpParameter : ICSharpNode
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ParameterSyntax Syntax { get; }
    CSharpSyntaxNode ICSharpNode.Syntax => Syntax;

    public IReadOnlyList<CSharpAttribute> Attributes { get; }
    public CSharpModifiers Modifiers { get; }
    public CSharpType Type { get; }
    public string Name { get; }
    public string? DefaultValue { get; }

    public CSharpParameter(ParameterSyntax syntax)
    {
        Syntax = syntax;
        Attributes = CSharpAttribute.Parse(syntax.AttributeLists);
        Modifiers = CSharpModifiersHelper.Parse(syntax.Modifiers);
        Type = new CSharpType(syntax.Type!);
        Name = syntax.Identifier.ToString();
        DefaultValue = syntax.Default?.Value?.ToString();
    }
}
