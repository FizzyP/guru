using System;
using System.Collections.Generic;

namespace Guru
{
	[Serializable]
	class SerializableRelatedItems : IManuallySerializable< RelatedItems >
	{
		//  readonly Dictionary<Item, HashSet<Item>> dependencyMap = new Dictionary<Item, HashSet<Item>>();
		public SerializableDictionary<Item, HashSet<Item>, SerializableHashSet2<Item>> serializableDependencyMap;
		//  readonly HashSet<Item> items = new HashSet<Item>();
		public SerializableHashSet2<Item> serializableItems;
		//  readonly HashSet<Item> doneItems = new HashSet<Item>();
		public SerializableHashSet2<Item> serializableDoneItems;
		//  Dictionary<UInt64, Item> itemByIdMap;
		public SerializableDictionary2<UInt64, Item> serializableItemByIdMap;

		public SerializableRelatedItems( RelatedItems item )
		{
			this.serializeFrom( item );
		}		
		
		public void serializeFrom( RelatedItems items )
		{
			serializableDependencyMap = new SerializableDictionary<Item, HashSet<Item>, SerializableHashSet2<Item>>();
			serializableDependencyMap.serializeFrom( items.dependencyMap );
			
			serializableItems = new SerializableHashSet2<Item>();
			serializableItems.serializeFrom( items.items );
			
			serializableDoneItems = new SerializableHashSet2<Item>();
			serializableDoneItems.serializeFrom( items.doneItems );
			
			serializableItemByIdMap = new SerializableDictionary2<UInt64, Item>();
			serializableItemByIdMap.serializeFrom( items.itemByIdMap );
		}
		
		public RelatedItems getDeserializedValue()
		{
			var items = new RelatedItems();
			
			items.dependencyMap = serializableDependencyMap.getDeserializedValue();
			items.items = serializableItems.getDeserializedValue();
			items.doneItems = serializableDoneItems.getDeserializedValue();
			items.itemByIdMap = serializableItemByIdMap.getDeserializedValue();
			
			return items;
		}
	}
}


