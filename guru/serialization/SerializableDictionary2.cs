using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guru
{
    [Serializable]
    class SerializableDictionary2<K, V> : IManuallySerializable<Dictionary<K, V>>
    {
        public List<K> Keys;
        public List<V> Values;

        public Dictionary<K, V> getDeserializedValue()
        {
            var dict = new Dictionary<K, V>();
            for (int i = 0; i < Keys.Count; i++)
            {
                dict[Keys[i]] = Values[i];
            }
            return dict;
        }

        public void serializeFrom(Dictionary<K, V> dict)
        {
            Keys = new List<K>();
            Values = new List<V>();

            foreach (var kv in dict)
            {
                Keys.Add(kv.Key);
                Values.Add(kv.Value);
            }
        }

    }
}
