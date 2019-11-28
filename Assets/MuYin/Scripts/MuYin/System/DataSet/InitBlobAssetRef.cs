using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MuYin
{
    [DisableAutoCreation]
    public class InitBlobAssetRef : ComponentSystem
    {
        protected override void OnUpdate()
        {
            InitializeBlobAssetRef();
            return;
        }

        private void InitializeBlobAssetRef()
        {
            var query = GetEntityQuery(typeof(ActionSetting), typeof(ItemSetting));
            var entities = query.ToEntityArray(Allocator.TempJob);
            Entities.ForEach((ref ActionDataSetContainer c0) =>
            {
                for (int i = 0; i < query.CalculateEntityCount(); i++)
                {
                    EntityManager.SetComponentData(entities[i], new ActionSetting
                    {
                        ActionDataSet = c0.ActionDataSet
                    });
                }
            });
            Entities.ForEach((ref ItemDataSetContainer c0) =>
            {
                for (int i = 0; i < query.CalculateEntityCount(); i++)
                {
                    EntityManager.SetComponentData(entities[i], new ItemSetting
                    {
                        ItemDataSet = c0.ItemDataSet
                    });
                }
            });
            entities.Dispose();
        }

        protected override void OnCreate() { }

        protected override void OnDestroy() { }
    }
}
