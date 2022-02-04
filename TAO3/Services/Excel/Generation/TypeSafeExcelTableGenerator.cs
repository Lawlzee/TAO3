using Microsoft.DotNet.Interactive.CSharp;
using TAO3.CodeGeneration;
using TAO3.Internal.Extensions;
using TAO3.TypeProvider;

namespace TAO3.Excel.Generation;

internal class TypeSafeExcelTableGenerator
{
    private readonly ITypeProvider<ExcelTable> _typeProvider;

    public TypeSafeExcelTableGenerator(ITypeProvider<ExcelTable> typeProvider)
    {
        _typeProvider = typeProvider;
    }

    public string Generate(CSharpKernel cSharpKernel, ExcelTable table)
    {
        string rowTypeName = GenerateRowType(cSharpKernel, table);
        string tableTypeName = GenerateTableType(cSharpKernel, table, rowTypeName);
        return tableTypeName;
    }

    private string GenerateRowType(CSharpKernel cSharpKernel, ExcelTable table)
    {
        SchemaSerialization schema = _typeProvider.ProvideTypes(table);
        cSharpKernel.ScheduleSubmitCode(schema.Code);
        return _typeProvider.Serializer.PrettyPrint(schema.RootElementType!);
    }

    private string GenerateTableType(CSharpKernel cSharpKernel, ExcelTable table, string rowTypeName)
    {
        string className = IdentifierUtils.ToCSharpIdentifier(table.Name);

        string code = $@"using System;
using System.Collections.Generic;
using System.Linq;
using TAO3.Excel;

public class {className} : ExcelTable
{{
    internal {className}(ExcelTable table)
        : base(table)
    {{

    }}

    public List<{rowTypeName}> Get() => Get<{rowTypeName}>();

    public void Set(IEnumerable<{rowTypeName}> data) => Set<{rowTypeName}>(data);
}}";
        cSharpKernel.ScheduleSubmitCode(code);

        return className;
    }
}
