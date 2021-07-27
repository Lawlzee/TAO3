using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters.Csv
{
    public class CsvSource
    {
        public string RootTypeName { get; }
        public string Csv { get; }
        public CsvConfiguration Configuration { get; }

        public CsvSource(string rootTypeName, string csv, CsvConfiguration configuration)
        {
            RootTypeName = rootTypeName;
            Csv = csv;
            Configuration = configuration;
        }
    }
}
