using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Formatting
{
    public record TAO3Formatters(
        CSharpFormatter CSharp,
        JsonFormatter Json,
        SqlFormatter Sql,
        XmlFormatter Xml) : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
