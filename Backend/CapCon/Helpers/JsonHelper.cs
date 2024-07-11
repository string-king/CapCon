using System.Text.Json;

namespace Helpers;

public class JsonHelper
{
    public static JsonSerializerOptions CamelCase { get; set; } = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}