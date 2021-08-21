using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TypeProvider
{
    public class RenameEmptyClassNameTransformer : ISchemaTransformation
    {
        public void Transform(ClassSchemaReplacor replacor)
        {
            for (int i = 0; i < replacor.Classes.Count; i++)
            {
                ClassSchema clazz = replacor.Classes[i];
                if (string.IsNullOrWhiteSpace(clazz.Identifier))
                {
                    ClassSchema replacement = new ClassSchema(
                        clazz.FullName,
                        "Class" + i,
                        clazz.Properties);

                    replacor.ReplaceWith(clazz, replacement);
                }
            }
        }
    }
}
