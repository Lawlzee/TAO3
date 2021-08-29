using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.TypeProvider;

namespace TAO3.Converters
{
    public record InferedType(IDomType Type, bool ReturnTypeIsList = false)
    {
    }

    public interface IInputTypeProvider<TSettings, TCommandParameters> : IInputConfigurableConverterCommand<TSettings, TCommandParameters>
    {
        IDomCompiler DomCompiler { get; }
        Task<InferedType> ProvideTypeAsync(IConverterContext<TSettings> context, TCommandParameters args);
    }
}
