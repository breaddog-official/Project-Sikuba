using Newtonsoft.Json;
using UnityEngine;

namespace Scripts.SaveManagement
{
    [CreateAssetMenu(fileName = "SerializerJson", menuName = "Scripts/Serializer")]
    public class SerializerJson : Serializer
    {
        public readonly JsonSerializerSettings serializerSettings = null;

        public SerializerJson(JsonSerializerSettings settings)
        {
            serializerSettings = settings;
        }


        public override string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, serializerSettings);
        }

        public override object Deserialize(string value)
        {
            return JsonConvert.DeserializeObject(value, serializerSettings);
        }

        public override T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, serializerSettings);
        }
    }
}
