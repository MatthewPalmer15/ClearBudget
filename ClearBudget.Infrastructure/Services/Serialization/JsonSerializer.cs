using Newtonsoft.Json;

namespace ClearBudget.Infrastructure.Services.Serialization;

public interface IJsonSerializer
{
    string Serialize(object obj);
    object? Deserialize(string obj);
    T? Deserialize<T>(string obj);
}

internal class JsonSerializer : IJsonSerializer
{
    public string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public object? Deserialize(string obj)
    {
        return JsonConvert.DeserializeObject(obj);
    }

    public T? Deserialize<T>(string obj)
    {
        return JsonConvert.DeserializeObject<T>(obj);
    }
}