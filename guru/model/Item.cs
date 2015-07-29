using System;

namespace Guru
{
	[Serializable]
	class Item : IManuallySerializable<Item>
	{
		public UInt64 Id;		
		public string Description;
		public string Details;
		public bool IsDone = false;
        public double Rank;
		
        
        		
		public Item(UInt64 id, string description, string details = null) {
			this.Id = id;
			this.Description = description;
			this.Details = details;
            this.Rank = 0;
		}
		
		public void copyFrom(Item item)
		{
			Id = item.Id;
			Description = item.Description;
			Details = item.Details;
			IsDone = item.IsDone;
            Rank = item.Rank;
		}
		
		public override string ToString()
		{
            const int fixedWidth = 5;

            var idStr = Id.ToString();
            if (idStr.Length < fixedWidth - 1)
            {
                idStr += " - "; // + new string(' ', fixedWidth - idStr.Length);
            }
            else
            {
                idStr += " - ";
            }

            return idStr + Description;
//				+ (Details == null ? "" : " : " + Details);
		}

        public string ToDetailedString()
        {
            return ToString() + (Details == null ? "" : " : " + System.Environment.NewLine + Details);
        }
		
		public void complete()
		{
			this.IsDone = true;
		}

		public Item getDeserializedValue() {
            //var copy = new Item();
            //copy.copyFrom(this);
            //return copy;
            return this;
		}
		
		public void serializeFrom(Item value)
		{
			this.copyFrom(value);
		}

	}
}

