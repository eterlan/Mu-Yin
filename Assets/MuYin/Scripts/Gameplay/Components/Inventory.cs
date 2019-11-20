using MuYin.Gameplay.Enum;
using Unity.Entities;


namespace MuYin.Gameplay.Components
{
    public struct Inventory : IBufferElementData
    {
        public ItemType ItemType;
    }
    
}