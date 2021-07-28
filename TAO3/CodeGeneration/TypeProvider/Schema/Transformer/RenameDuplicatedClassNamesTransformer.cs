using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.CodeGeneration;

namespace TAO3.TypeProvider
{
    public class RenameDuplicatedClassNamesTransformer : ISchemaTransformation
    {
        public void Transform(ClassSchemaReplacor replacor)
        {
            HashSet<string> identifiersUsed = new HashSet<string>();

            for (int i = 0; i < replacor.Classes.Count; i++)
            {
                ClassSchema clazz = replacor.Classes[i];
                if (identifiersUsed.Contains(clazz.Identifier))
                {
                    string newIdentifier = IdentifierUtils.GetUniqueIdentifier(clazz.Identifier, identifiersUsed);

                    ClassSchema replacement = new ClassSchema(
                        clazz.FullName,
                        newIdentifier,
                        clazz.Properties);

                    replacor.ReplaceWith(clazz, replacement);

                    identifiersUsed.Add(newIdentifier);
                }
                else
                {
                    identifiersUsed.Add(clazz.Identifier);
                }
            }
        }
    }
}
