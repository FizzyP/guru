using System;
using System.Collections.Generic;

namespace Guru
{
	//	K = key type
	//	V = value type
	//	SV = serialized value

	[Serializable]
	class SerializableDictionary<K, V, SV> : IManuallySerializable<Dictionary<K, V>> where SV : IManuallySerializable<V>, new()
	{
		public List<K> Keys;
		public List<SV> SerializedValues;

		public Dictionary<K, V> getDeserializedValue()
		{
			var dict = new Dictionary<K, V>();
			for (int i=0; i < Keys.Count; i++) {
				dict[ Keys[i] ] = SerializedValues[i].getDeserializedValue();
			}
			return dict;
		}
		
		public void serializeFrom(Dictionary<K, V> dict)
		{
			Keys = new List<K>();
			SerializedValues = new List<SV>();
			
			foreach (var kv in dict) {
				Keys.Add( kv.Key );
				var sv = new SV();
				sv.serializeFrom( kv.Value );
				SerializedValues.Add( sv );
			}
		}
				
	}
}