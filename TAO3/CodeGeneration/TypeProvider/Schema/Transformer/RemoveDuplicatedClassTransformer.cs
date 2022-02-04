namespace TAO3.TypeProvider;

public class RemoveDuplicatedClassTransformer : ISchemaTransformation
{
    public void Transform(ClassSchemaReplacor replacor)
    {
        for (int i = 0; i < replacor.Classes.Count; i++)
        {
            for (int j = i + 1; j < replacor.Classes.Count; j++)
            {
                ClassSchema classI = replacor.Classes[i];
                ClassSchema classJ = replacor.Classes[j];

                if (classI.AreEquivalent(classJ))
                {
                    replacor.SubstituteWith(j, i);
                    j--;
                }
            }
        }
    }
}
