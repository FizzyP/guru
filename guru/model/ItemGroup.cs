using System;
using System.Collections.Generic;

namespace Guru
{
	[Serializable]
	class ItemGroup
	{
		public UInt64 nextId = 1;
		public RelatedItems Items = new RelatedItems();
		
		public IEnumerable<Item> EnumerableItems {
			get {
				return Items.EnumerableItems;
			}
		}

        public IEnumerable<Item> EnumerableDoneItems
        {
            get
            {
                return Items.EnumerableDoneItems;
            }
        }

        public UInt64 allocateId() {
			return nextId++;			
		}
		
		public Item getItemById(UInt64 id)
		{
			return Items.getItemById(id);
		}
	}
}

