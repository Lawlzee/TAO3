using System.CommandLine;

namespace TAO3.Converters
{
    public interface IConfigurableConverter
    {
        void ConfigureCommand(Command command, ConvertionContextProvider contextProvider);
    }
}
