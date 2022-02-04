using TAO3.CodeGeneration;

namespace TAO3.TypeProvider;

public class RenameDuplicatedPropertyNamesTransformer : ISchemaTransformation
{
    public void Transform(ClassSchemaReplacor replacor)
    {
        for (int i = 0; i < replacor.Classes.Count; i++)
        {
            ClassSchema clazz = replacor.Classes[i];
            HashSet<string> identifiersUsed = new HashSet<string>
            {
                clazz.Identifier
            };

            bool hasRenamedProperty = false;

            List<ClassPropertySchema> properties = new List<ClassPropertySchema>();
            foreach (ClassPropertySchema property in clazz.Properties)
            {
                if (identifiersUsed.Contains(property.Identifier))
                {
                    hasRenamedProperty = true;
                    string newIdentifier = IdentifierUtils.GetUniqueIdentifier(property.Identifier, identifiersUsed);

                    properties.Add(new ClassPropertySchema(
                        newIdentifier,
                        property.FullName,
                        property.Type));


                    identifiersUsed.Add(newIdentifier);
                }
                else
                {
                    properties.Add(property);
                    identifiersUsed.Add(property.Identifier);
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
