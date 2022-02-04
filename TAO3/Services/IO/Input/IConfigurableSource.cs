using System.CommandLine;

namespace TAO3.IO;

public interface IConfigurableSource
{
    void Configure(Command command);
}
