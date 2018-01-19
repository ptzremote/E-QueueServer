using Newtonsoft.Json;
using System.Text;

namespace EQueueLib
{
    public static class JSONSerializerHelper
    {
        static JsonSerializerSettings jSonSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        public static byte[] Serialize(object data)
        {
            string output = JsonConvert.SerializeObject(data, jSonSettings);

            return BytesFromString(output);
        }

        public static object Deserialize(byte[] bytes)
        {
            return JsonConvert.DeserializeObject(StringFromBytes(bytes), jSonSettings);
        }

        static byte[] BytesFromString(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
        static string StringFromBytes(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
