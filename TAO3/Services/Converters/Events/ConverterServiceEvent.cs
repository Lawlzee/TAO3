namespace TAO3.Converters;

public interface IConverterEvent
{
    IConverter Converter { get; }
}

public record ConverterRegisteredEvent(IConverter Converter) : IConverterEvent;
public record ConverterUnregisteredEvent(IConverter Converter) : IConverterEvent;
