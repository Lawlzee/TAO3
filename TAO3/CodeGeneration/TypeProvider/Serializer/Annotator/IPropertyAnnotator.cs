using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TypeProvider
{
    public interface IPropertyAnnotator
    {
        void Annotate(ClassPropertySchema property, AnnotatorContext context);
    }
}
