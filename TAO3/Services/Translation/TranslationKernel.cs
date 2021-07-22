using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Translation
{
    internal class TranslationKernel : Kernel, IKernelCommandHandler<SubmitCode>
    {
        private readonly Translator _translator;

        public TranslationKernel(Translator translator, string name) : base(name)
        {
            _translator = translator;
        }

        public async Task HandleAsync(SubmitCode command, KernelInvocationContext context)
        {
            string? translation = await _translator.TranslateAsync(command.Code);
            translation.Display();
        }
    }
}
