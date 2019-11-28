using Unity.Entities;
using UnityEngine;

namespace MuYin
{
    public struct Inventory : IBufferElementData
    {
        public ItemType ItemType;
        public uint ItemId;
        public byte Capacity;
        public byte Count;
    }

    [RequiresEntityConversion]
    [RequireComponent(typeof(ConvertToEntity))]
    [DisallowMultipleComponent]
    public class InventoryAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert( Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
        {
            manager.AddBuffer<Inventory>(entity);
        }
    }
    
}