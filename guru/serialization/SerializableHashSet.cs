using System;
using System.Collections.Generic;

namespace Guru
{
	// V = value
	// SV = serialized version of value
	[Serializable]
	class SerializableHashSet<V, SV> : IManuallySerializable< HashSet<V> > where SV : IManuallySerializable< V >, new()
	{
		List<SV> serializedValues;

		public void serializeFrom( HashSet<V> hashSet )
		{
			serializedValues = new List<SV>();
			foreach (var x in hashSet) {
				var sv = new SV();
				sv.serializeFrom(x);
				serializedValues.Add( sv );
			}
		}
		
		public HashSet<V> getDeserializedValue()
		{
			var hashSet = new HashSet<V>();

			foreach (var x in serializedValues) {
				hashSet.Add( x.getDeserializedValue() );
			}
			
			return hashSet;
		}
	}
}

