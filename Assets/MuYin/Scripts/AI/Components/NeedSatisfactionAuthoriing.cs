using Unity.Entities;
using UnityEngine;

namespace MuYin.AI.Components
{
    public struct NeedSatisfaction : IComponentData
    {
        public int  CurrentLv;
        public bool Satisfied;
    }
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class NeedSatisfactionAuthoriing : MonoBehaviour, IConvertGameObjectToEntity
    {
        public int  LV;
        public bool Satisfied;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new NeedSatisfaction
            {
                CurrentLv = LV,
                Satisfied = Satisfied,
            };
            dstManager.AddComponentData(entity, data);
        
        }
    }
}