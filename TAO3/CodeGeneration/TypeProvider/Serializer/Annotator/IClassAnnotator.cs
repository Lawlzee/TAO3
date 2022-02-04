namespace TAO3.TypeProvider;

public interface IClassAnnotator
{
    void Annotate(ClassSchema clazz, ClassAnnotatorContext context);
}
