namespace TAO3.TypeProvider;

public interface IDomParser<TInput>
{
    IDomType Parse(TInput input);
}
