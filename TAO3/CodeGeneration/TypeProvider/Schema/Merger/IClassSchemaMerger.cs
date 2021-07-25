namespace TAO3.TypeProvider
{
    public interface IClassSchemaMerger
    {
        static IClassSchemaMerger Default { get; } = SchemaMerger.Default;
        ClassSchema MergeClasses(ClassSchema classA, ClassSchema classB);
    }
}
