using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using MuYin.Gameplay;
using MuYin.Gameplay.Systems;
using MuYin.AI.Action.ActionData;
using MuYin.AI.Enum;
using MuYin.AI.Components;
using System.Collections.Generic;

namespace MuYin.GameManager
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
            Debug.Log("111");
            World.GetOrCreateSystem<InitOwnerSystem>().Update();
            World.GetOrCreateSystem<InitActionLookUpTable>().Update();
        }

        protected override void OnCreate()
        {
            World.GetOrCreateSystem<InitActionLookUpTable>().OnCreate();
        }

        protected override void OnDestroy() { }
    }
}
