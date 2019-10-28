using Unity.Entities;
using UnityEngine;

namespace MuYin.Gameplay.Components
{
    public struct Bed : IComponentData
    {
        public int RestorationValue;
        public int SleepTime;
    }
    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public class BedAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public int RestorationValue;
        public int SleepTime;

        public void Convert( Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
        {
            var data = new Bed
            {
                RestorationValue = RestorationValue,
                // TODO: Set Editor.
                SleepTime = SleepTime,
            };
            manager.AddComponentData(entity, data);
        }
    }
}