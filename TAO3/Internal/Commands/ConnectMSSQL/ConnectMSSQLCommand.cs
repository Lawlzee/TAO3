using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.SqlServer;
using Microsoft.DotNet.Interactive.Utility;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Runtime.InteropServices;
using TAO3.Internal.Extensions;

namespace TAO3.Internal.Commands.ConnectMSSQL;

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

    //Microsoft.DotNet.Interactive.SqlServer.Utils
    private class DotnetToolInfo
    {
        public string PackageId { get; set; }
        public string PackageVersion { get; set; }
        public string CommandName { get; set; }

    }

    private async Task<Kernel> CreateKernelAsync(
        string connectionString,
        string kernelName,
        KernelInvocationContext context,
        bool verbose)
    {
        var sqlToolName = "MicrosoftSqlToolsServiceLayer";
        await CheckAndInstallGlobalToolAsync(sqlToolName, "1.2.0", "Microsoft.SqlServer.SqlToolsServiceLayer.Tool");
        var sqlToolPath = Path.Combine(Paths.DotnetToolsPath, sqlToolName);

        MsSqlKernelConnector connector = new MsSqlKernelConnector(createDbContext: false, connectionString)
        {
            PathToService = sqlToolName
        };

        Kernel kernel = await connector.CreateKernelAsync(kernelName);

        await InitializeDbContextAsync(connectionString, kernelName, context, verbose);

        return kernel;

        //Microsoft.DotNet.Interactive.SqlServer.Utils
        async Task CheckAndInstallGlobalToolAsync(string toolName, string minimumVersion, string nugetPackage)
        {
            var installedGlobalTools = await GetGlobalToolListAsync();
            var expectedVersion = Version.Parse(minimumVersion);
            var installNeeded = true;
            var updateNeeded = false;
            foreach (var tool in installedGlobalTools)
            {
                if (string.Equals(tool.CommandName, toolName, StringComparison.InvariantCultureIgnoreCase))
                {
                    installNeeded = false;
                    var installedVersion = Version.Parse(tool.PackageVersion);
                    if (installedVersion < expectedVersion)
                    {
                        updateNeeded = true;
                    }
                    break;
                }
            }

            var dotnet = new Microsoft.DotNet.Interactive.Utility.Dotnet();
            if (updateNeeded)
            {
                var commandLineResult = await dotnet.Execute($"tool update --global \"{nugetPackage}\" --version \"{minimumVersion}\"");
                commandLineResult.ThrowOnFailure();
            }
            else if (installNeeded)
            {
                var commandLineResult = await dotnet.Execute($"tool install --global \"{nugetPackage}\" --version \"{minimumVersion}\"");
                commandLineResult.ThrowOnFailure();
            }
        }

        //Microsoft.DotNet.Interactive.SqlServer.Utils
        async Task<IEnumerable<DotnetToolInfo>> GetGlobalToolListAsync()
        {
            var dotnet = new Microsoft.DotNet.Interactive.Utility.Dotnet();
            var result = await dotnet.Execute("tool list --global");
            if (result.ExitCode != 0)
            {
                return new DotnetToolInfo[0];
            }

            // Output of dotnet tool list is:
            // Package Id        Version      Commands
            // -------------------------------------------
            // dotnettry.p1      1.0.0        dotnettry.p1

            string[] separator = new[] { " " };
            return result.Output
                .Skip(2)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s =>
                {
                    var parts = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    return new DotnetToolInfo()
                    {
                        PackageId = parts[0],
                        PackageVersion = parts[1],
                        CommandName = parts[2]
                    };
                });
        }
    }

    private async Task InitializeDbContextAsync(
        string connectionString,
        string kernelName,
        KernelInvocationContext context,
        bool verbose)
    {
        CSharpKernel csharpKernel = context.GetCSharpKernel();

        //$"Scaffolding a `DbContext` and initializing an instance of it called `{kernelName}` in the C# kernel.".Display("text/markdown");

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

code += System.Text.RegularExpressions.Regex.Replace(
    CleanFile(model.ContextFile.Code),
    @""protected override void OnConfiguring\(DbContextOptionsBuilder optionsBuilder\)
#warning .*
        => (.*)"",
    @""protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {{
        $1
        _interceptor = new TAO3.Internal.Commands.ConnectMSSQL.SaveChangesInterceptor();
        optionsBuilder.AddInterceptors(_interceptor);
    }}"");

foreach (var file in model.AdditionalFiles.Select(f => f.Code))
{{              
    code += CleanFile(file);
}}

string CleanFile(string file)
{{
    var namespaceToFind = ""namespace {kernelName};"";
    var headerSize = file.LastIndexOf(namespaceToFind)  + namespaceToFind.Length;
    var fileCode = file
        // remove namespaces, which don't compile in Roslyn scripting
        .Substring(headerSize).Trim();

    return fileCode;
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
