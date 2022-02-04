using Microsoft.CodeAnalysis.CSharp;
using RazorLight.Compilation;
using RazorLight.Generation;
using System.Reflection;

namespace TAO3.Internal.Kernels.Razor;

internal static class RazorLightExtensions
{
    private static readonly FieldInfo _razorSourceGeneratorFieldInfo = typeof(RazorTemplateCompiler)
        .GetField("_razorSourceGenerator", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly MethodInfo _createCompilationMethodInfo = typeof(RoslynCompilationService).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(x => x.GetParameters().Length == 2)
        .First(x => x.Name == "CreateCompilation");

    public static RazorSourceGenerator GetSourceGenerator(this IRazorTemplateCompiler compiler)
    {
        return (RazorSourceGenerator)_razorSourceGeneratorFieldInfo.GetValue(compiler)!;
    }

    public static CSharpCompilation CreateCompilation(this ICompilationService compilationService, string compilationContent, string assemblyName)
    {
        return (CSharpCompilation)_createCompilationMethodInfo.Invoke(compilationService, new[] { compilationContent, assemblyName })!;
    }
}
