using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.TypeProvider;

namespace TAO3.Converters.Sql
{
    public class TableNameAnnotator : IClassAnnotator
    {
        public void Annotate(ClassSchema clazz, ClassAnnotatorContext context)
        {
            context.Using(typeof(TableNameAttribute).Namespace!);
            context.StringBuilder.Append(@$"[TableName(""{clazz.FullName.Replace("\"", "\"\"")}"")]
");
        }
    }
}
