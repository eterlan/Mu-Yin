using Unity.Entities;
using UnityEngine;

namespace MuYin
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(EventInvokerGroup))]
    public class GameManager : ComponentSystem
    {
        protected override void OnUpdate()
        {
 
        }

        protected override void OnStartRunning()
        {
            Debug.Log("ManagerStartRunning");
            World.GetOrCreateSystem<InitBlobAssetRef>().Update();
            World.GetOrCreateSystem<InitOwnerSystem>().Update();
            //World.GetOrCreateSystem<InitActionLookUpTable>().Update();
        }

        protected override void OnCreate()
        {
            //World.GetOrCreateSystem<InitActionLookUpTable>().OnCreate();
        }

        protected override void OnDestroy() { }
    }
}
