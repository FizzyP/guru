using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guru
{

    [Serializable]
    class SerializableHashSet2<V> : IManuallySerializable<HashSet<V>>
    {
        List<V> values;

        public void serializeFrom(HashSet<V> hashSet)
        {
            values = new List<V>();
            foreach (var x in hashSet)
            {
                values.Add(x);
            }
        }

        public HashSet<V> getDeserializedValue()
        {
            var hashSet = new HashSet<V>();

            foreach (var x in values)
            {
                hashSet.Add(x);
            }

            return hashSet;
        }
    }
}
