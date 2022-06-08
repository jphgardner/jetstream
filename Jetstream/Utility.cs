using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Jetstream;

public class Utility
{
    public static string AssemblyDirectory
    {
        get
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }

    public static byte[] Encode<T>(T data) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
    public static T Decode<T>(string data) => JsonConvert.DeserializeObject<T>(data);

    public static T Decode<T>(object data) => JsonConvert.DeserializeObject<T>(data.ToString());
    
    public static object Decode(object data) => JsonConvert.DeserializeObject(data.ToString());

    public static object Decode(byte[] data, Type type) =>
        JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), type) ?? throw new InvalidOperationException();

    public static string Base64Encode(string plainText) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

    public static string Base64Encode(object @object) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@object)));

    public static string Base64Decode(string base64EncodedData) =>
        Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));

    public static T Decode<T>(byte[] data) => JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data),
        new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error
        }) ?? throw new InvalidOperationException();
}