using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Guru
{
	[Serializable]
	class RelatedItems
	{
		
		#region State
		
		//	Each item is associated with a list of other items it depends on
		public Dictionary<Item, HashSet<Item>> dependencyMap = new Dictionary<Item, HashSet<Item>>();
		public HashSet<Item> items = new HashSet<Item>();
		public HashSet<Item> doneItems = new HashSet<Item>();
		public Dictionary<UInt64, Item> itemByIdMap;
		
		#endregion
		
		public RelatedItems()
		{
			itemByIdMap = new Dictionary<UInt64, Item>();
		}
			
		public IEnumerable<Item> EnumerableItems {
			get {
				return (IEnumerable<Item>) items;
			}
		}


        public IEnumerable<Item> EnumerableDoneItems
        {
            get
            {
                return (IEnumerable<Item>)doneItems;
            }
        }

        public ObjectForest getItemsAsForest(HashSet<Item> itemSet)
        {
            var forest = new ObjectForest();

            var rootNodes = new HashSet<ObjectForestNode>();
            var itemToNodeMap = new Dictionary<Item, ObjectForestNode>();

            foreach (var i in itemSet)
            {
                var node = new ObjectForestNode();
                node.Value = i;
                rootNodes.Add(node);
                itemToNodeMap[i] = node;
            }

            foreach (var kv in dependencyMap)
            {
                var parent = kv.Key;
                foreach (var item in kv.Value)
                {
                    if (!itemSet.Contains(item))
                        continue;

                    var childNode = itemToNodeMap[item];
                    itemToNodeMap[parent].Children.Add(childNode);
                    //  Remember the child isn't a root
                    rootNodes.Remove(childNode);
                }
            }

            //  Write the root nodes
            foreach (var root in rootNodes)
                forest.addRoot(root);

            return forest;
        }

        public ObjectForest getItemsAsForest()
        {
            return getItemsAsForest(this.items);
        }

        public ObjectForest getDoneItemsAsForest()
        {
            return getItemsAsForest(this.doneItems);
        }

        //public ObjectForest getItemsAsForest()
        //{
        //    var forest = new ObjectForest();

        //    var rootNodes = new HashSet<ObjectForestNode>();
        //    var itemToNodeMap = new Dictionary<Item, ObjectForestNode>();

        //    foreach (var i in items)
        //    {
        //        var node = new ObjectForestNode();
        //        node.Value = i;
        //        rootNodes.Add(node);
        //        itemToNodeMap[i] = node;
        //    }

        //    foreach (var kv in dependencyMap)
        //    {
        //        var parent = kv.Key;
        //        foreach (var item in kv.Value)
        //        {
        //            if (!items.Contains(item))
        //                continue;

        //            var childNode = itemToNodeMap[item];
        //            itemToNodeMap[parent].Children.Add(childNode);
        //            //  Remember the child isn't a root
        //            rootNodes.Remove(childNode);
        //        }
        //    }

        //    //  Write the root nodes
        //    foreach (var root in rootNodes)
        //        forest.addRoot(root);

        //    return forest;
        //}


        public void addItem(Item item)
		{
			if (items.Contains(item))
				return;
				
			items.Add( item );
			itemByIdMap.Add(item.Id, item);
		}
		
		public void addDependency(Item item, Item dependency)
		{
			if ( !dependencyMap.ContainsKey(item) ) {
				dependencyMap[item] = new HashSet<Item>();
			}
			
			var deps = dependencyMap[item];
			if (deps.Contains(dependency))
				return;
			
			deps.Add(dependency);
		}
		
		public List<Item> getFreeItems()
		{
			var freeItems = items
				.Where( 	i => !this.itemHasIncopmleteDependencies(i) );
//				.OrderBy(	i => i.Id );

			return freeItems.ToList();
		}
		
		public int ItemCount {
			get {
				return items.Count();
			}
		}
		
		public int DoneItemCount {
			get {
				return doneItems.Count();
			}
		}
		
		public Item getTopItem()
		{
			var freeItems = getFreeItems();
			if (freeItems.Count() == 0)
				return null;
				
			return freeItems[0];
		}
		
		
		private bool itemHasIncopmleteDependencies(Item item)
		{
			if (!dependencyMap.ContainsKey( item ) )
				return false;
			
			var deps = dependencyMap[ item ];
			foreach(var dep in deps) {
				if (!dep.IsDone) {
					return true;
				}
			}
			
			return false;
		}
		
		
		public Item getItemById(UInt64 id)
		{
			return itemByIdMap[id];
		}
		
		
		public List<Item> findItemsWithDescriptionMatchingRegex(Regex regex)
		{
			var matches = new List<Item>();
			
			foreach (var item in items) {
				if (regex.IsMatch(item.Description)) {
					matches.Add(item);
				}
			}
			return matches;
		}
		
		
		public void HashSetsOnDeserialization()
		{
			items.OnDeserialization(this);
			doneItems.OnDeserialization(this);
			dependencyMap.OnDeserialization(this);
			Console.WriteLine("dependencyMap " + dependencyMap.Count());
			foreach (var kv in dependencyMap) {
				Console.WriteLine("this");
				kv.Value.OnDeserialization(this);
			}
		}
		
	}
}