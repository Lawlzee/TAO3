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
            AddOption(new Option<string>(new[] { "--connectionString" }, "Connection String"));
            AddOption(new Option<string>(new[] { "--kernelName" }, "Kernel Name"));
            AddOption(new Option<int>(new[] { "--minPoolSize" }, "Min Pool Size"));
            AddOption(new Option<bool>(new[] { "--multipleActiveResultSets" }, "Multiple Active Result Sets"));
            AddOption(new Option<bool>(new[] { "--multiSubnetFailover" }, "Multi Subnet Failover"));
            AddOption(new Option<int>(new[] { "--packetSize" }, "Packet Size"));
            AddOption(new Option<string>(new[] { "--password" }, "Password"));
            AddOption(new Option<string>(new[] { "--pwd" }, "PWD"));
            AddOption(new Option<bool>(new[] { "--persistSecurityInfo" }, "Persist Security Info"));
            AddOption(new Option<bool>(new[] { "--pooling" }, "Pooling"));
            AddOption(new Option<bool>(new[] { "--replication" }, "Replication"));
            AddOption(new Option<string>(new[] { "--transactionBinding" }, "Transaction Binding"));
            AddOption(new Option<bool>(new[] { "--trustServerCertificate" }, "Trust Server Certificate"));
            AddOption(new Option<string>(new[] { "--typeSystemVersion" }, "Type System Version"));
            AddOption(new Option<string>(new[] { "--userID" }, "User ID"));
            AddOption(new Option<string>(new[] { "--user" }, "User"));
            AddOption(new Option<string>(new[] { "--uid" }, "UID"));
            AddOption(new Option<bool>(new[] { "--userInstance" }, "User Instance"));
            AddOption(new Option<string>(new[] { "--workstationID" }, "Workstation ID"));
            AddOption(new Option<string>(new[] { "--wsid" }, "WSID"));
            AddOption(new Option<PoolBlockingPeriod>(new[] { "--poolBlockingPeriod" }, "Pool Blocking Period"));
            AddOption(new Option<SqlConnectionColumnEncryptionSetting>(new[] { "--columnEncryptionSetting" }, "Column Encryption Setting"));
            AddOption(new Option<int>(new[] { "--maxPoolSize" }, "Max Pool Size"));
            AddOption(new Option<int>(new[] { "--connectionLifetime" }, "Connection Lifetime"));
            AddOption(new Option<int>(new[] { "--loadBalanceTimeout" }, "Load Balance Timeout"));
            AddOption(new Option<SqlConnectionAttestationProtocol>(new[] { "--attestationProtocol" }, "Attestation Protocol"));
            AddOption(new Option<string>(new[] { "--enclaveAttestationUrl" }, "Enclave Attestation Url"));
            AddOption(new Option<ApplicationIntent>(new[] { "--applicationIntent" }, "Application Intent"));
            AddOption(new Option<string>(new[] { "--applicationName" }, "Application Name"));
            AddOption(new Option<string>(new[] { "--app" }, "App"));
            AddOption(new Option<string>(new[] { "--attachDBFilename" }, "AttachDbFilename"));
            AddOption(new Option<string>(new[] { "--extendedProperties" }, "Extended Properties"));
            AddOption(new Option<string>(new[] { "--initialFileName" }, "Initial File Name"));
            AddOption(new Option<SqlAuthenticationMethod>(new[] { "--authentication" }, "Authentication"));
            AddOption(new Option<int>(new[] { "--connectRetryCount" }, "Connect Retry Count"));
            AddOption(new Option<int>(new[] { "--connectTimeout" }, "Connect Timeout"));
            AddOption(new Option<int>(new[] { "--connectionTimeout" }, "Connection Timeout"));
            AddOption(new Option<int>(new[] { "--timeout" }, "Timeout"));
            AddOption(new Option<string>(new[] { "--currentLanguage" }, "Current Language"));
            AddOption(new Option<string>(new[] { "--language" }, "Language"));
            AddOption(new Option<int>(new[] { "--connectRetryInterval" }, "Connect Retry Interval"));
            AddOption(new Option<bool>(new[] { "--encrypt" }, "Encrypt"));
            AddOption(new Option<bool>(new[] { "--enlist" }, "Enlist"));
            AddOption(new Option<string>(new[] { "--failoverPartner" }, "Failover Partner"));
            AddOption(new Option<string>(new[] { "--initialCatalog" }, "Initial Catalog"));
            AddOption(new Option<string>(new[] { "--database" }, "Database"));
            AddOption(new Option<bool>(new[] { "--integratedSecurity" }, "Integrated Security"));
            AddOption(new Option<bool>(new[] { "--trustedConnection" }, "Trusted Connection"));
            AddOption(new Option<string>(new[] { "--dataSource" }, "Data Source"));
            AddOption(new Option<string>(new[] { "--server" }, "Server"));
            AddOption(new Option<string>(new[] { "--address" }, "Address"));
            AddOption(new Option<string>(new[] { "--addr" }, "Addr"));
            AddOption(new Option<string>(new[] { "--networkAddress" }, "Network Address"));

            Handler = CommandHandler.Create(async (ConnectMSSQLOptions options) =>
            {
                Kernel kernel = await CreateKernelAsync(
                    options.GetConnectionString(), 
                    options.GetkernelName(), 
                    options.Context);

                options.Context.HandlingKernel.ParentKernel.Add(kernel);
            });
        }

        private async Task<Kernel> CreateKernelAsync(
            string connectionString,
            string kernelName,
            KernelInvocationContext context)
        {
            Kernel kernel = await new MsSqlKernelConnection().CreateKernelAsync(new MsSqlConnectionOptions
            {
                ConnectionString = connectionString,
                CreateDbContext = false,
                KernelName = kernelName
            }, context);


            await InitializeDbContextAsync(connectionString, kernelName, context);

            return kernel;
        }

        private async Task InitializeDbContextAsync(
            string connectionString,
            string kernelName,
            KernelInvocationContext context)
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
            await csharpKernel.SubmitCodeAsync(submission1);

            csharpKernel.TryGetVariable("code", out string submission2);

            await csharpKernel.SubmitCodeAsync(submission2);

            var submission3 = $@"
var {kernelName} = new {kernelName}Context();";

            await csharpKernel.SubmitCodeAsync(submission3);
        }
    }
}
