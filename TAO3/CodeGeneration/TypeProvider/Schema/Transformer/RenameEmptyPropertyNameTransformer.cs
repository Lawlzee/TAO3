using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TypeProvider
{
    public class RenameEmptyPropertyNameTransformer : ISchemaTransformation
    {
        public void Transform(ClassSchemaReplacor replacor)
        {
            for (int i = 0; i < replacor.Classes.Count; i++)
            {
                ClassSchema clazz = replacor.Classes[i];
                bool hasRenamedProperty = false;

                List<ClassPropertySchema> properties = new List<ClassPropertySchema>();
                foreach (ClassPropertySchema property in clazz.Properties)
                {
                    if (string.IsNullOrWhiteSpace(property.Identifier))
                    {
                        hasRenamedProperty = true;
                        string newIdentifier = "Prop" + properties.Count;

                        properties.Add(new ClassPropertySchema(
                            newIdentifier,
                            property.FullName,
                            property.Type));
                    }
                    else
                    {
                        properties.Add(property);
                    }
                }

                if (hasRenamedProperty)
                {
                    ClassSchema replacement = new ClassSchema(
                        clazz.FullName,
                        clazz.Identifier,
                        properties);

                    replacor.ReplaceWith(clazz, replacement);
                }
            }
        }
    }
}
