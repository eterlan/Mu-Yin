using Unity.Entities;
using UnityEngine;


namespace MuYin.GameManager
{
    public struct GameInfo : IComponentData
    {
        // Todo: Should change to event.
        public bool Game_First_Initialization;
    }

    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public class GameInfoAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert
        (
            Entity                     entity,
            EntityManager              manager,
            GameObjectConversionSystem conversionSystem)
        {
            manager.CreateEntity(typeof(GameInfo));
            var singletonGroup  = manager.CreateEntityQuery(typeof(GameInfo));
            var data = new GameInfo { };
            singletonGroup.SetSingleton(data);
        }
    }
}