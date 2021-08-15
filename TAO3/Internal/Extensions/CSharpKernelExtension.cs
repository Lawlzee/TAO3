using Microsoft.CodeAnalysis;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TAO3.Internal.Extensions
{
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
            return (Task)_runAsyncMethodInfo.Invoke(kernel, new object[] { code, cancellationToken, catchException })!;
        }
    }
}
