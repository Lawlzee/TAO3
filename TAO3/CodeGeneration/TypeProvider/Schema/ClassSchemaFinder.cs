namespace TAO3.TypeProvider;

public class ClassSchemaFinder : SchemaVisitor
{
    private readonly List<ClassSchema> _classesFound;

    private ClassSchemaFinder()
    {
        _classesFound = new List<ClassSchema>();
    }

    public static List<ClassSchema> FindClasses(ISchema schema)
    {
        ClassSchemaFinder classFinder = new ClassSchemaFinder();
        schema.Accept(classFinder);
        return classFinder._classesFound
            .Distinct()
            .ToList();
    }

    public override void Visit(ClassSchema node)
    {
        _classesFound.Add(node);
        base.Visit(node);
    }
}
