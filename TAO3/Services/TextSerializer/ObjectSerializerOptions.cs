using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TAO3.TextSerializer
{
    public class ObjectSerializerOptions
    {
        public int IndentationLevel { get; private set; }
        public string IndentationString { get; private set; }
        public string Indentation { get; private set; } 

        public ObjectSerializerOptions(int indentationLevel = 0, string indentationString = "    ")
        {
            IndentationLevel = indentationLevel;
            IndentationString = indentationString;
            Indentation = string.Concat(Enumerable.Repeat(IndentationString, IndentationLevel));
        }

        public ObjectSerializerOptions Indent()
        {
            return With(x =>
            {
                x.IndentationLevel += 1;
                x.Indentation = string.Concat(Enumerable.Repeat(x.IndentationString, x.IndentationLevel));
            });
        }

        private ObjectSerializerOptions With(Action<ObjectSerializerOptions> setValues)
        {
            ObjectSerializerOptions clone = (ObjectSerializerOptions)MemberwiseClone();
            setValues(clone);
            return clone;
        }
    }
}
