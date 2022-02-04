namespace TAO3.TypeProvider;

public interface IPropertyAnnotator
{
    void Annotate(ClassPropertySchema property, PropertyAnnotatorContext context);
}
