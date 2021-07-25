using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TypeProvider
{
    public interface IClassAnnotator
    {
        void Annotate(ClassSchema clazz, AnnotatorContext context);
    }
}
