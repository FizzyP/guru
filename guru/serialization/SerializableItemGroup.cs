
using System;

namespace Guru
{
	[Serializable]
	class SerializableItemGroup : IManuallySerializable< ItemGroup >
	{
		public UInt64 nextId;
		public SerializableRelatedItems SerializableItems;

		public void serializeFrom( ItemGroup group )
		{
			nextId = group.nextId;
			SerializableItems = new SerializableRelatedItems( group.Items );
		}
		
		public ItemGroup getDeserializedValue()
		{
			var group = new ItemGroup();
			group.nextId = nextId;
			group.Items = SerializableItems.getDeserializedValue();
			return group;
		}

	}
}


