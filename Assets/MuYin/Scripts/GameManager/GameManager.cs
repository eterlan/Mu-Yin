using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using MuYin.Gameplay;
using MuYin.Gameplay.Systems;

namespace MuYin.GameManager
{
    [UpdateInGroup(typeof(EventInvokerGroup))]
    public class GameManager : ComponentSystem
    {
        protected override void OnUpdate()
        {
 
        }

        protected override void OnStartRunning()
        {
            World.GetOrCreateSystem<InitOwnerSystem>().Update();
        }

        protected override void OnCreate()
        {
        }

        protected override void OnDestroy() { }
    }
}
