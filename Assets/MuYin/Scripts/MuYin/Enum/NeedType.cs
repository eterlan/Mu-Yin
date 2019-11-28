namespace MuYin
{
    public enum NeedType
    {
        Null,
        Hungry,
        Thirst,
        Sleepness,
        Food,
        Water,
        Scavenge,
    }

    public static class NeedTypeMethod
    {
        public static ItemType GetMappingItemType(this NeedType needType)
        {
            switch (needType)
            {
                case NeedType.Hungry : return ItemType.Food;
                case NeedType.Food : return ItemType.Food;
                case NeedType.Thirst : return ItemType.Water;
                case NeedType.Water : return ItemType.Water;

                default : return ItemType.Null;
            }
        }
    }
}