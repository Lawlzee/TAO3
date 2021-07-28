namespace TAO3.TypeProvider
{
    public interface ITypeReferenceSchemaMerger
    {
        static ITypeReferenceSchemaMerger Default { get; } = new SchemaMerger(ILiteralTypeMerger.Default);
        TypeReferenceSchema Merge(TypeReferenceSchema a, TypeReferenceSchema b);
    }
}
