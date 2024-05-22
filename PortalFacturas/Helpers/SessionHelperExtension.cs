using System.Text.Json;

using Microsoft.AspNetCore.Http;

namespace PortalFacturas.Helpers;

public static class SessionHelperExtension
{
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T GetObjectFromJson<T>(this ISession session, string key)
    {
        string value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}
