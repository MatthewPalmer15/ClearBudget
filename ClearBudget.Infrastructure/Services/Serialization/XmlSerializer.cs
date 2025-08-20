namespace ClearBudget.Infrastructure.Services.Serialization;

public interface IXmlSerializer
{
    string Serialize(object obj);
    T? Deserialize<T>(string xml);
}

internal class XmlSerializer : IXmlSerializer
{
    public string Serialize(object obj)
    {
        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, obj);
        return stringWriter.ToString();
    }

    public T? Deserialize<T>(string xml)
    {
        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        using var stringReader = new StringReader(xml);
        return (T?)xmlSerializer.Deserialize(stringReader);
    }
}