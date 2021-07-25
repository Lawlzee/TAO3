using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TypeProvider
{
    public interface ITypeSchema : ISchema
    {
        bool IsValueType { get; }

        new ITypeSchema Accept(SchemaRewriter rewriter);
        ISchema ISchema.Accept(SchemaRewriter rewriter) => Accept(rewriter);
    }
}
