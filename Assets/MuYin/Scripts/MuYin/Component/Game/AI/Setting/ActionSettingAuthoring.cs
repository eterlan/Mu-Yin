using Unity.Entities;
using UnityEngine;


namespace MuYin
{
    public struct ActionSetting : IComponentData
    {
        public BlobAssetReference<ActionDataSet> ActionDataSet;
    }

    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    [DisallowMultipleComponent]
    public class ActionSettingAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert
        (
            Entity                     entity,
            EntityManager              manager,
            GameObjectConversionSystem conversionSystem)
        {
            manager.AddComponent<ActionSetting>(entity);
            Debug.Log("ConvertActionSetting");
        }
    }
}