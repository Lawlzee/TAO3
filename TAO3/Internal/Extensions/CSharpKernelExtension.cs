using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System.Reflection;
using System.Threading;

namespace TAO3.Internal.Extensions;

internal static class CSharpKernelExtension
{
    private readonly static MethodInfo _ensureWorkspaceIsInitializedAsyncMethodInfo = typeof(CSharpKernel)
        .GetMethod("EnsureWorkspaceIsInitializedAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private readonly static FieldInfo _workspaceFieldInfo = typeof(CSharpKernel)
        .GetField("_workspace", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private readonly static MethodInfo _forkDocumentForLanguageServicesMethodInfo = _workspaceFieldInfo.FieldType
        .GetMethod("ForkDocumentForLanguageServices", BindingFlags.Public | BindingFlags.Instance)!;

    private readonly static MethodInfo _runAsyncMethodInfo = typeof(CSharpKernel)
        .GetMethod("RunAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;

    public static Task EnsureWorkspaceIsInitializedAsync(this CSharpKernel kernel, KernelInvocationContext context)
    {
        return (Task)_ensureWorkspaceIsInitializedAsyncMethodInfo.Invoke(kernel, new[] { context })!;
    }

    public static Document ForkDocumentForLanguageServices(this CSharpKernel kernel, string code)
    {
        object? workspace = _workspaceFieldInfo.GetValue(kernel);
        return (Document)_forkDocumentForLanguageServicesMethodInfo.Invoke(workspace, new[] { code })!;
    }

    public static Task RunAsync(
        this CSharpKernel kernel,
        string code,
        CancellationToken cancellationToken = default,
        Func<Exception, bool>? catchException = default)
    {
        return (Task)_runAsyncMethodInfo.Invoke(kernel, new object?[] { code, cancellationToken, catchException })!;
    }

    public static async Task<string> CreatePrivateVariableAsync(this CSharpKernel csharpKernel, object? value, Type type)
    {
        string name = $"__internal_{Guid.NewGuid().ToString("N")}";
        await csharpKernel.SetValueAsync(name, value, type);
        return name;
    }

    public static Task<KernelCommandResult> SubmitCodeAsync(this Kernel kernel, string code, bool verbose)
    {
        if (verbose)
        {
            code.Display();
        }

        return kernel.SubmitCodeAsync(code);
    }

    public static Task<bool> IsCompleteSubmissionAsync(this CSharpKernel kernel, string code)
    {
        CSharpParseOptions options = new(LanguageVersion.Latest, kind: SourceCodeKind.Script);
        var syntaxTree = SyntaxFactory.ParseSyntaxTree(code, options);
        return Task.FromResult(SyntaxFactory.IsCompleteSubmission(syntaxTree));
    }
}
