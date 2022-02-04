using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Kernels.Razor
{
    internal class ChooseRazorKernelDirective : ChooseKernelDirective
    {
        public Option<string> MimeTypeOption { get; }
        public Option<string> NameOption { get; }
        public Option<bool> SuppressOption { get; }
        public Option<bool> VerboseOption { get; }

        public ChooseRazorKernelDirective(
            Kernel kernel, 
            string? description = null) : base(kernel, description)
        {
            MimeTypeOption = new Option<string>(new[] { "--mimeType", "-m" }, "Mime type used to display the resulting document");
            NameOption = new Option<string>(new[] { "--name", "-n" }, "Name of the variable that will containt the resulting document");
            SuppressOption = new Option<bool>(new[] { "--suppress", "-s" }, "Suppress the displaying of the resulting document");
            VerboseOption = new Option<bool>(new[] { "--verbose", "-v" }, "Print debugging information");

            MimeTypeOption.AddCompletions(
                "text/html",
                "text/plain",
                "application/json",
                "text/markdown",
                "text/x-csharp",
                "text/x-javascript",
                "text/x-fsharp",
                "text/x-sql",
                "text/x-powershell");

            Add(MimeTypeOption);
            Add(NameOption);
            Add(SuppressOption);
            Add(VerboseOption);
        }

        protected override async Task Handle(KernelInvocationContext kernelInvocationContext, InvocationContext commandLineInvocationContext)
        {
            SubmitCode submitCode = (SubmitCode)kernelInvocationContext.Command;
            RazorKernel razorKernel = (RazorKernel)Kernel;
            RazorOptions options = RazorOptions.Create(commandLineInvocationContext.ParseResult, this);

            await razorKernel.HandleAsync(submitCode, kernelInvocationContext, options);
        }
    }

    internal record RazorOptions(
        string? MimeType,
        string? Name,
        bool Suppress,
        bool Verbose)
    { 

        public static RazorOptions Create(ParseResult parseResult, ChooseRazorKernelDirective directive)
        {
            return new RazorOptions(
                parseResult.GetValueForOption(directive.MimeTypeOption),
                parseResult.GetValueForOption(directive.NameOption),
                parseResult.GetValueForOption(directive.SuppressOption),
                parseResult.GetValueForOption(directive.VerboseOption));
        }
    }
}
