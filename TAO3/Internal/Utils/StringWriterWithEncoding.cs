using System.IO;

namespace TAO3.Internal.Utils;

internal class StringWriterWithEncoding : StringWriter
{
    private readonly Encoding _encoding;
    public override Encoding Encoding => _encoding;

    public StringWriterWithEncoding(Encoding encoding)
    {
        _encoding = encoding;
    }

    public StringWriterWithEncoding(Encoding encoding, IFormatProvider? formatProvider) 
        : base(formatProvider)
    {
        _encoding = encoding;
    }

    public StringWriterWithEncoding(Encoding encoding, StringBuilder sb) 
        : base(sb)
    {
        _encoding = encoding;
    }

    public StringWriterWithEncoding(Encoding encoding, StringBuilder sb, IFormatProvider? formatProvider) 
        : base(sb, formatProvider)
    {
        _encoding = encoding;
    }
}
