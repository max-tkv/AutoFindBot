using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AutoFindBot.Utils.Http;

/// <summary>
/// Базовый класс API клиент для работы по http/json
/// </summary>
public abstract class JsonHttpApiClient : HttpApiClient
{
    private static readonly JsonSerializerSettings JsonSerializerSettings;

    static JsonHttpApiClient()
    {
        JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };
    }

    protected JsonHttpApiClient(HttpClient httpClient) :
        base(httpClient)
    {
    }

    protected override HttpContent GetContent(object obj)
    {
        var content = JsonConvert.SerializeObject(obj, JsonSerializerSettings);
        return new StringContent(content, Encoding.UTF8, "application/json");
    }
    
    protected override HttpContent GetContent(string text)
    {
        return new StringContent(text, Encoding.UTF8, "text/plain");
    }
    
    protected override HttpContent GetContent(List<KeyValuePair<string, string>> nvc)
    {
        return new FormUrlEncodedContent(nvc);
    }

    protected override T GetObjectFromResponse<T>(string content)
    {
        return JsonConvert.DeserializeObject<T>(content);
    }
}