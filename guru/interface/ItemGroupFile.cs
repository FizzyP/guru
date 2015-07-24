
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Guru
{
	class ItemGroupFile
	{
		public readonly string Path;
		public readonly ItemGroup ItemGroup;
		
		private ItemGroupFile(string path, ItemGroup group) {
			Path = path;
			ItemGroup = group;
		}
		
		public static ItemGroupFile new_CreateAtPath(string path)
		{
			return new ItemGroupFile(path, new ItemGroup());
		}
		
		public static ItemGroupFile new_ReadFromPath(string path)
		{
			IFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			SerializableItemGroup sGroup = (SerializableItemGroup) formatter.Deserialize(stream);
			stream.Close();
			
			ItemGroup group = sGroup.getDeserializedValue();			
			
			return new ItemGroupFile(path, group);
		}
		
		public void save()
		{
			//	Package the data to store
			var serializedItemGroup = new SerializableItemGroup();
			serializedItemGroup.serializeFrom( this.ItemGroup );

			IFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.None);
			formatter.Serialize(stream, serializedItemGroup);
			stream.Close();
		}
	}
}
