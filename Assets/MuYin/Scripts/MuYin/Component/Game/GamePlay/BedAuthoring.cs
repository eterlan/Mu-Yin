using Unity.Entities;
using UnityEngine;

namespace MuYin
{
    public struct Bed : IComponentData
    {
        public int Restoration;
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
                Restoration = RestorationValue,
                // TODO: Set Editor.
                SleepTime = SleepTime,
            };
            manager.AddComponentData(entity, data);
        }
    }
}