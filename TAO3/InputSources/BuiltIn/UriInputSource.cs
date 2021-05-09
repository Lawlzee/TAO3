using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.InputSources
{
    internal class UriInputSource : IInputSource
    {
        public string Name => "uri";

        public async Task<string> GetText(string source, KernelInvocationContext context)
        {
            var client = new HttpClient();
            return await client.GetStringAsync(source);
        }
    }
}
