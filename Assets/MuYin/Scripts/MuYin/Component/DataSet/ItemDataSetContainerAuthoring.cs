using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MuYin
{
    public struct ItemDataSetContainer : IComponentData
    {
        public BlobAssetReference<ItemDataSet> ItemDataSet;
    }

    [RequiresEntityConversion]
    [RequireComponent(typeof(ConvertToEntity))]
    [DisallowMultipleComponent]
    public class ItemDataSetContainerAuthoring : SerializedMonoBehaviour, IConvertGameObjectToEntity
    {
        public ItemData[] ItemDataSet;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new ItemDataSetContainer
            {
                ItemDataSet = BuildItemDataSet(ItemDataSet)
            };
            dstManager.AddComponentData(entity, data);
        }

        private BlobAssetReference<ItemDataSet> BuildItemDataSet(ItemData[] srcItemData)
        {
            using (var builder = new BlobBuilder(Allocator.Temp))
            {
                ref var root = ref builder.ConstructRoot<ItemDataSet>();
                var dstItemData = builder.Allocate(ref root.ItemDatas, srcItemData.Length);
                for (var i = 0; i < dstItemData.Length; i++)
                {
                    dstItemData[i] = srcItemData[i];
                }
                return builder.CreateBlobAssetReference<ItemDataSet>(Allocator.Persistent);
            }
        }
    }  

    public struct ItemDataSet
    {
        public BlobArray<ItemData> ItemDatas;
    }

    public struct ItemData
    {
        public uint ItemID;
        public ItemType ItemType;
        public byte CapacityPerGrid;
        public sbyte EffectValue;
        public byte ApplyTimes;
        // public uint Price;
        // public byte EffectID;
        // 
        public ItemData(uint itemID, ItemType itemType, byte capacityPerGrid) : this()
        {
            ItemID = itemID; 
            ItemType = itemType;
            CapacityPerGrid = capacityPerGrid;
        }
    }  
}
