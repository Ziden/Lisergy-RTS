using Newtonsoft.Json;

namespace WebGameLogic
{
    /// <summary>
    /// Just a central point for serialization of web objects.
    /// Will replace later by binary base64 representation for smaller objects
    /// </summary>
    public static class WebSerializer
    {
        public static T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value)!;
        }

        public static string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}
