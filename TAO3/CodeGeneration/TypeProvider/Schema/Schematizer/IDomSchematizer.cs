using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TypeProvider
{
    public interface IDomSchematizer
    {
        static IDomSchematizer Default { get; } = new DomSchematizer(IDomReducer.Default, ISchemaCleaner.Default);
        DomSchema Schematize(IDomType node, string format);
    }

    public class DomSchematizer : IDomSchematizer
    {
        private readonly IDomReducer _domReducer;
        private readonly ISchemaCleaner _schemaCleaner;

        public DomSchematizer(IDomReducer domReducer, ISchemaCleaner schemaCleaner)
        {
            _domReducer = domReducer;
            _schemaCleaner = schemaCleaner;
        }

        public DomSchema Schematize(IDomType node, string format)
        {
            ITypeSchema schema = _domReducer.Reduce(node);
            DomSchema domSchema = _schemaCleaner.Clean(schema, format);
            return domSchema;
        }
    }
}
