using System.Text.Json;

namespace WebApp.Helpers;

public class JsonHelper
{
    public static JsonSerializerOptions CamelCase = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
