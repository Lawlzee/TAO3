using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.SqlServer;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;

namespace TAO3.Internal.Commands.ConnectMSSQL
{
    internal class ConnectMSSQLCommand : Command
    {
        public ConnectMSSQLCommand()
            : base("#!connectMSSQL", "Connects to a Microsoft SQL Server database")
        {
            Add(new Argument<string?>("connectionString", () => null, "Connection String"));
            Add(new Option<string>(new[] { "--kernelName" }, "Kernel Name"));
            Add(new Option<int>(new[] { "--minPoolSize" }, "Min Pool Size"));
            Add(new Option<bool>(new[] { "--multipleActiveResultSets" }, "Multiple Active Result Sets"));
            Add(new Option<bool>(new[] { "--multiSubnetFailover" }, "Multi Subnet Failover"));
            Add(new Option<int>(new[] { "--packetSize" }, "Packet Size"));
            Add(new Option<string>(new[] { "--password" }, "Password"));
            Add(new Option<string>(new[] { "--pwd" }, "PWD"));
            Add(new Option<bool>(new[] { "--persistSecurityInfo" }, "Persist Security Info"));
            Add(new Option<bool>(new[] { "--pooling" }, "Pooling"));
            Add(new Option<bool>(new[] { "--replication" }, "Replication"));
            Add(new Option<string>(new[] { "--transactionBinding" }, "Transaction Binding"));
            Add(new Option<bool>(new[] { "--trustServerCertificate" }, "Trust Server Certificate"));
            Add(new Option<string>(new[] { "--typeSystemVersion" }, "Type System Version"));
            Add(new Option<string>(new[] { "--userID" }, "User ID"));
            Add(new Option<string>(new[] { "--user" }, "User"));
            Add(new Option<string>(new[] { "--uid" }, "UID"));
            Add(new Option<bool>(new[] { "--userInstance" }, "User Instance"));
            Add(new Option<string>(new[] { "--workstationID" }, "Workstation ID"));
            Add(new Option<string>(new[] { "--wsid" }, "WSID"));
            Add(new Option<PoolBlockingPeriod>(new[] { "--poolBlockingPeriod" }, "Pool Blocking Period"));
            Add(new Option<SqlConnectionColumnEncryptionSetting>(new[] { "--columnEncryptionSetting" }, "Column Encryption Setting"));
            Add(new Option<int>(new[] { "--maxPoolSize" }, "Max Pool Size"));
            Add(new Option<int>(new[] { "--connectionLifetime" }, "Connection Lifetime"));
            Add(new Option<int>(new[] { "--loadBalanceTimeout" }, "Load Balance Timeout"));
            Add(new Option<SqlConnectionAttestationProtocol>(new[] { "--attestationProtocol" }, "Attestation Protocol"));
            Add(new Option<string>(new[] { "--enclaveAttestationUrl" }, "Enclave Attestation Url"));
            Add(new Option<ApplicationIntent>(new[] { "--applicationIntent" }, "Application Intent"));
            Add(new Option<string>(new[] { "--applicationName" }, "Application Name"));
            Add(new Option<string>(new[] { "--app" }, "App"));
            Add(new Option<string>(new[] { "--attachDBFilename" }, "AttachDbFilename"));
            Add(new Option<string>(new[] { "--extendedProperties" }, "Extended Properties"));
            Add(new Option<string>(new[] { "--initialFileName" }, "Initial File Name"));
            Add(new Option<SqlAuthenticationMethod>(new[] { "--authentication" }, "Authentication"));
            Add(new Option<int>(new[] { "--connectRetryCount" }, "Connect Retry Count"));
            Add(new Option<int>(new[] { "--connectTimeout" }, "Connect Timeout"));
            Add(new Option<int>(new[] { "--connectionTimeout" }, "Connection Timeout"));
            Add(new Option<int>(new[] { "--timeout" }, "Timeout"));
            Add(new Option<string>(new[] { "--currentLanguage" }, "Current Language"));
            Add(new Option<string>(new[] { "--language" }, "Language"));
            Add(new Option<int>(new[] { "--connectRetryInterval" }, "Connect Retry Interval"));
            Add(new Option<bool>(new[] { "--encrypt" }, "Encrypt"));
            Add(new Option<bool>(new[] { "--enlist" }, "Enlist"));
            Add(new Option<string>(new[] { "--failoverPartner" }, "Failover Partner"));
            Add(new Option<string>(new[] { "--initialCatalog" }, "Initial Catalog"));
            Add(new Option<string>(new[] { "--database" }, "Database"));
            Add(new Option<bool>(new[] { "--integratedSecurity" }, "Integrated Security"));
            Add(new Option<bool>(new[] { "--trustedConnection" }, "Trusted Connection"));
            Add(new Option<string>(new[] { "--dataSource" }, "Data Source"));
            Add(new Option<string>(new[] { "--server" }, "Server"));
            Add(new Option<string>(new[] { "--address" }, "Address"));
            Add(new Option<string>(new[] { "--addr" }, "Addr"));
            Add(new Option<string>(new[] { "--networkAddress" }, "Network Address"));
            Add(new Option<bool>(new[] { "--verbose", "-v" }, "Print debugging information"));

            Handler = CommandHandler.Create(async (ConnectMSSQLOptions options, bool verbose) =>
            {
                Kernel kernel = await CreateKernelAsync(
                    options.GetConnectionString(), 
                    options.GetkernelName(), 
                    options.Context,
                    verbose);

                options.Context.HandlingKernel.ParentKernel.Add(kernel);
            });
        }

        private async Task<Kernel> CreateKernelAsync(
            string connectionString,
            string kernelName,
            KernelInvocationContext context, 
            bool verbose)
        {
            Kernel kernel = await new MsSqlKernelConnection().CreateKernelAsync(new MsSqlConnectionOptions
            {
                ConnectionString = connectionString,
                CreateDbContext = false,
                KernelName = kernelName
            }, context);


            await InitializeDbContextAsync(connectionString, kernelName, context, verbose);

            return kernel;
        }

        private async Task InitializeDbContextAsync(
            string connectionString,
            string kernelName,
            KernelInvocationContext context,
            bool verbose)
        {
            CSharpKernel csharpKernel = context.GetCSharpKernel();

            context.Display($"Scaffolding a `DbContext` and initializing an instance of it called `{kernelName}` in the C# kernel.", "text/markdown");

            var submission1 = @$"
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
    @""{connectionString}"",
    new DatabaseModelFactoryOptions(),
    new ModelReverseEngineerOptions(),
    new ModelCodeGenerationOptions()
    {{
        ContextName = ""{kernelName}Context"",
        ModelNamespace = ""{kernelName}""
    }});

var code = @""using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Metadata;

public partial class {kernelName}Context
{{
    private TAO3.Internal.Commands.ConnectMSSQL.SaveChangesInterceptor _interceptor;
    public List<string> SqlQueries => _interceptor.SqlQueries;

    partial void CustomOnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {{
        _interceptor = new TAO3.Internal.Commands.ConnectMSSQL.SaveChangesInterceptor();
        optionsBuilder.AddInterceptors(_interceptor);
    }}

    public List<string> GetSaveScript()
    {{
        using (IDbContextTransaction transaction = Database.BeginTransaction())
        {{
            try
            {{
                SaveChanges();
                return _interceptor.SqlQueries;
            }}
            finally
            {{
                transaction.Rollback();
            }}
        }}
    }}

    public async Task<List<string>> GetSaveScriptAsync()
    {{
        using (IDbContextTransaction transaction = await Database.BeginTransactionAsync())
        {{
            try
            {{
                await SaveChangesAsync();
                return _interceptor.SqlQueries;
            }}
            finally
            {{
                transaction.Rollback();
            }}
        }}
    }}
}}
"";

code += CleanFile(model.ContextFile.Code)
    .Replace(
        @""protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {{"",
        @""
        partial void CustomOnConfiguring(DbContextOptionsBuilder optionsBuilder);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {{
            CustomOnConfiguring(optionsBuilder);"");

foreach (var file in model.AdditionalFiles.Select(f => f.Code))
{{              
    code += CleanFile(file);
}}

string CleanFile(string file)
{{
    return file
        // remove namespaces, which don't compile in Roslyn scripting
        .Replace(""namespace {kernelName}"", """")

        // remove the namespaces, which have been hoisted to the top of the code submission
        .Replace(""using System;"", """")
        .Replace(""using System.Collections.Generic;"", """")
        .Replace(""using Microsoft.EntityFrameworkCore;"", """")
        .Replace(""using Microsoft.EntityFrameworkCore.Metadata;"", """")

        // trim out the wrapping braces
        .Trim()
        .Trim( new[] {{ '{{', '}}' }} );
}}
";
            await SubmitCodeAsync(submission1);

            csharpKernel.TryGetValue("code", out string submission2);

            await SubmitCodeAsync(submission2);

            var submission3 = $@"
var {kernelName} = new {kernelName}Context();";

            await SubmitCodeAsync(submission3);

            async Task SubmitCodeAsync(string code)
            {
                if (verbose)
                {
                    code.Display();
                }

                await csharpKernel.SubmitCodeAsync(code);
            }
        }
    }
}
