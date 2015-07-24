
namespace Guru
{
	interface IManuallySerializable<V>
	{
		V getDeserializedValue();
		void serializeFrom(V value);
	}
}