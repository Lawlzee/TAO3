using Humanizer;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TAO3.Internal.Commands.GenerateHttpClient
{
    internal class GenerateHttpClientCommand : Command
    {
        private enum EndPointType
        {
            Svc,
            Asmx
        }

        public GenerateHttpClientCommand(CSharpKernel csharpKernel)
            : base("#!generateHttpClient", "Generate a client for a svc or asmx endpoint")
        {
            Add(new Argument<Uri>("uri", "URI of the svc or asmx endpoint"));
            Add(new Option<string>(new[] { "--clientName", "-c" }, "Name of the instance of the generated client"));
            Add(new Option<bool>(new[] { "--verbose", "-v" }, "Print debugging information"));

            string? svcUtilPath = null;

            Handler = CommandHandler.Create(async (Uri uri, string clientName, bool verbose) =>
            {
                if (svcUtilPath == null)
                {
                    string directory = Path.Combine(Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%"), @"Microsoft SDKs\Windows");
                    svcUtilPath = Directory.GetFiles(directory, "SvcUtil.exe", SearchOption.AllDirectories).LastOrDefault();
                    if (svcUtilPath == null)
                    {
                        throw new Exception("Path to SvcUtil.exe not found");
                    }

                    await SubmitCodeAsync(@"
#r nuget:System.ServiceModel.Http
#r nuget:System.ServiceModel.Primitives", verbose);
                }

                string temporaryFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
                string arguments = @$"""{uri}"" /noConfig /o:""{temporaryFile}"" /namespace:*,";
                await Process.Start(svcUtilPath, arguments).WaitForExitAsync();

                string code = File.ReadAllText(temporaryFile);
                File.Delete(temporaryFile);

                await SubmitCodeAsync(code, verbose);

                EndPointType endPointType = InferEndPointType(uri);

                string? clientClassName = InferClientClassName(code);
                if (clientClassName == null)
                {
                    @$"Could not infer client name. Use {CreateInstantiateClient(uri,"MyClient", endPointType)} to initialise the client manually".Display();
                    return;
                }

                string actualClientName = string.IsNullOrEmpty(clientName)
                    ? CreateDefaultClientName(clientClassName, endPointType)
                    : clientName;

                string initialiseClient = $@"
{clientClassName} {actualClientName} = {CreateInstantiateClient(uri, clientClassName, endPointType)}";

                await SubmitCodeAsync(initialiseClient, verbose);

                $"{actualClientName} generated".Display();
            });

            async Task SubmitCodeAsync(string code, bool verbose)
            {
                if (verbose)
                {
                    code.Display();
                }

                await csharpKernel.SubmitCodeAsync(code);
            }

            EndPointType InferEndPointType(Uri uri)
            {
                string lastPart = uri.Segments.Last();
                if (lastPart.EndsWith(".svc", StringComparison.OrdinalIgnoreCase))
                {
                    return EndPointType.Svc;
                }

                return EndPointType.Asmx;
            }

            string? InferClientClassName(string code)
            {
                Match match = Regex.Match(code, $@"^public partial class (.*)Client : System\.ServiceModel\.ClientBase<.*>", RegexOptions.Multiline);
                if (match.Success)
                {
                    return match.Groups[1].Value + "Client";
                }

                return null;
            }

            string CreateDefaultClientName(string clientClassName, EndPointType endPointType)
            {
                if (endPointType == EndPointType.Asmx)
                {
                    return Regex.Replace(clientClassName, "SoapClient$", "Client").Camelize();
                }

                return clientClassName.Camelize();
            }

            string CreateInstantiateClient(Uri uri, string clientClassName, EndPointType endPointType)
            {
                return $@"new {clientClassName}(new System.ServiceModel.BasicHttpBinding(), new System.ServiceModel.EndpointAddress(@""{uri}{(endPointType == EndPointType.Svc ? "/soap" : "")}""));";
            }
        }
    }
}
