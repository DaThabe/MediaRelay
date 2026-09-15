using System.Text.Json.Serialization.Metadata;

namespace MediaRelay.Serialization;


public interface IJsonSerializerFactory
{
    ISerializer<T> Create<T>(JsonTypeInfo<T> jsonTypeInfo) where T : notnull;
}