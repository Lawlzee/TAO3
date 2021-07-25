namespace TAO3.TypeProvider
{
    public interface ITypeReferenceSchemaMerger
    {
        static ITypeReferenceSchemaMerger Default { get; } = SchemaMerger.Default;
        TypeReferenceSchema Merge(TypeReferenceSchema a, TypeReferenceSchema b);
    }
}
