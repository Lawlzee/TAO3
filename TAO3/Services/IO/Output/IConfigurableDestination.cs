using System.CommandLine;

namespace TAO3.IO;

public interface IConfigurableDestination
{
    void Configure(Command command);
}
