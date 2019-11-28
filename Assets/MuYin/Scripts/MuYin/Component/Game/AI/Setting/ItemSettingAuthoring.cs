using Unity.Entities;
using UnityEngine;


namespace MuYin
{
    public struct ItemSetting : IComponentData
    {
        public BlobAssetReference<ItemDataSet> ItemDataSet;
    }

    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    [DisallowMultipleComponent]
    public class ItemSettingAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert
        (
            Entity                     entity,
            EntityManager              manager,
            GameObjectConversionSystem conversionSystem)
        {
            manager.AddComponent<ItemSetting>(entity);
            Debug.Log("ConvertItemSetting");

        }
    }
}