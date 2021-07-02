﻿using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Connection;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.SqlServer;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Commands.Sql
{
    internal class TAO3MsSqlKernelConnection : ConnectKernelCommand<MsSqlConnectionOptions>
    {
        public TAO3MsSqlKernelConnection()
            : base("mssql2", "Connects to a Microsoft SQL Server database")
        {
            Add(new Argument<string>(
                    "connectionString",
                    "The connection string used to connect to the database"));
        }

        public override async Task<Kernel> CreateKernelAsync(
            MsSqlConnectionOptions options,
            KernelInvocationContext context)
        {
            Kernel kernel = await new MsSqlKernelConnection().CreateKernelAsync(new MsSqlConnectionOptions
            {
                ConnectionString = options.ConnectionString,
                CreateDbContext = false,
                KernelName = options.KernelName
            }, context);

            if (options.CreateDbContext)
            {
                await InitializeDbContextAsync(options, context);
            }

            return kernel;
        }

        private async Task InitializeDbContextAsync(MsSqlConnectionOptions options, KernelInvocationContext context)
        {
            CSharpKernel csharpKernel = null!;

            context.HandlingKernel.VisitSubkernelsAndSelf(k =>
            {
                if (k is CSharpKernel csk)
                {
                    csharpKernel = csk;
                }
            });

            if (csharpKernel is null)
            {
                return;
            }

            context.Display($"Scaffolding a `DbContext` and initializing an instance of it called `{options.KernelName}` in the C# kernel.", "text/markdown");

            var submission1 = @$"
#r ""nuget:Microsoft.EntityFrameworkCore.Design,3.1.8""
#r ""nuget:Microsoft.EntityFrameworkCore.SqlServer,3.1.8""

using System;
using System.Reflection;
using System.Linq;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddEntityFrameworkDesignTimeServices();
var providerAssembly = Assembly.Load(""Microsoft.EntityFrameworkCore.SqlServer"");
var providerServicesAttribute = providerAssembly.GetCustomAttribute<DesignTimeProviderServicesAttribute>();
var providerServicesType = providerAssembly.GetType(providerServicesAttribute.TypeName);
var providerServices = (IDesignTimeServices)Activator.CreateInstance(providerServicesType);
providerServices.ConfigureDesignTimeServices(services);

var serviceProvider = services.BuildServiceProvider();
var scaffolder = serviceProvider.GetService<IReverseEngineerScaffolder>();

var model = scaffolder.ScaffoldModel(
    @""{options.ConnectionString}"",
    new DatabaseModelFactoryOptions(),
    new ModelReverseEngineerOptions(),
    new ModelCodeGenerationOptions()
    {{
        ContextName = ""{options.KernelName}Context"",
        ModelNamespace = ""{options.KernelName}""
    }});

var code = @""using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;"";

foreach (var file in  new[] {{ model.ContextFile.Code }}.Concat(model.AdditionalFiles.Select(f => f.Code)))
{{
    var fileCode = file
        // remove namespaces, which don't compile in Roslyn scripting
        .Replace(""namespace {options.KernelName}"", """")

        // remove the namespaces, which have been hoisted to the top of the code submission
        .Replace(""using System;"", """")
        .Replace(""using System.Collections.Generic;"", """")
        .Replace(""using Microsoft.EntityFrameworkCore;"", """")
        .Replace(""using Microsoft.EntityFrameworkCore.Metadata;"", """")

        // trim out the wrapping braces
        .Trim()
        .Trim( new[] {{ '{{', '}}' }} );
                      
    code += fileCode;
}}
";

            await csharpKernel.SubmitCodeAsync(submission1);

            csharpKernel.TryGetVariable("code", out string submission2);

            await csharpKernel.SubmitCodeAsync(submission2);

            var submission3 = $@"
var {options.KernelName} = new {options.KernelName}Context();";

            await csharpKernel.SubmitCodeAsync(submission3);
        }
    }
}
