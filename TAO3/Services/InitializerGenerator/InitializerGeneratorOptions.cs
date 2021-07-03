using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TAO3.InitializerGenerator
{
    public class InitializerGeneratorOptions
    {
        public int IndentationLevel { get; private set; }
        public string IndentationString { get; private set; }
        public string Indentation { get; private set; } 

        public InitializerGeneratorOptions(int indentationLevel = 0, string indentationString = "    ")
        {
            IndentationLevel = indentationLevel;
            IndentationString = indentationString;
            Indentation = string.Concat(Enumerable.Repeat(IndentationString, IndentationLevel));
        }

        public InitializerGeneratorOptions Indent()
        {
            return With(x =>
            {
                x.IndentationLevel += 1;
                x.Indentation = string.Concat(Enumerable.Repeat(x.IndentationString, x.IndentationLevel));
            });
        }

        private InitializerGeneratorOptions With(Action<InitializerGeneratorOptions> setValues)
        {
            InitializerGeneratorOptions clone = (InitializerGeneratorOptions)MemberwiseClone();
            setValues(clone);
            return clone;
        }
    }
}
