﻿namespace TAO3.TypeProvider;

public class DomSchema
{
    public string Format { get; }
    public ISchema Schema { get; }
    public List<ClassSchema> Classes { get; }

    public DomSchema(string format, ISchema schema, List<ClassSchema> classes)
    {
        Format = format;
        Schema = schema;
        Classes = classes;
    }
}
