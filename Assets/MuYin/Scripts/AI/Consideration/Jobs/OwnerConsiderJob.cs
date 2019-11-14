using Unity.Collections;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using MuYin.Gameplay.Components;

namespace MuYin.AI.Consideration
{


    // // For each unoccupied place, consider each person's score.
    // [RequireComponentTag(typeof(Private)), ExcludeComponent(typeof(Owner))]
    // public struct OwnerConsider:IJobForEachWithEntity<SetPlaceOwnerConsiderer, Place, Translation>
    // {
    //     [DeallocateOnJobCompletion]
    //     public NativeArray<OwnerPosContainer> OwnersPos;
    //     [ReadOnly] public BufferFromEntity<MyOwnPlace> OwnPlacesBuffer;
    //     public EntityCommandBuffer.Concurrent BeginEcb;
    //     public EntityCommandBuffer.Concurrent EndEcb;
    //     public void Execute
    //     (
    //         Entity                      objectEntity,
    //         int                         index,
    //         ref SetPlaceOwnerConsiderer c0,
    //         ref Place                   c1,
    //         ref Translation             c2)
    //     {
    //         var maxScoreIndex = -1;
    //         Debug.Log(OwnersPos.Length);
    //         for (var i = 0; i < OwnersPos.Length; i++)
    //         {
    //             var ownerInfo = OwnersPos[i];
    //             if (ownerInfo.Occupied) continue;
    //             c0.Distance.Score = ConsiderDistance(i, ref c0, ref c2);
    //             c0.SamePlaceCount.Score = ConsiderPlaceCount(i, ref c0, ref c1);
                
    //             var score = (c0.Distance.Score + c0.SamePlaceCount.Score) / 2;
    //             //Debug.Log($"I am bed: {objectEntity}, this person{OwnersPos[i].OwnerEntity}'s score is {score}");
                
    //             if (c0.Score > score) continue;
                
    //             maxScoreIndex = i;
    //             c0.Score       = score;
    //             c0.OwnerEntity = OwnersPos[i].OwnerEntity;
    //         }

    //         if (maxScoreIndex == -1) return;
    //         var temp = OwnersPos[maxScoreIndex];
    //         temp.Occupied = true;
    //         OwnersPos[maxScoreIndex] = temp;
                
    //         BeginEcb.AddComponent(index, objectEntity, new SetPlaceOwnerEvent(c0.OwnerEntity, objectEntity, false));
    //         EndEcb.RemoveComponent<SetPlaceOwnerEvent>(index, objectEntity);
    //         EndEcb.RemoveComponent<SetPlaceOwnerConsiderer>(index, objectEntity);
    //     }

    //     private float ConsiderDistance(int i, ref SetPlaceOwnerConsiderer c0, ref Translation c2)
    //     {
    //         var distance    = math.distance(OwnersPos[i].Positions, c2.Value);
    //         return c0.Distance.Output(distance);
    //     }

    //     private float ConsiderPlaceCount(int i, ref SetPlaceOwnerConsiderer c0, ref Place c1)
    //     {
    //         var ownerEntity    = OwnersPos[i].OwnerEntity;
    //         var ownPlaces      = OwnPlacesBuffer[ownerEntity];
    //         var samePlaceCount = 0;
                
    //         foreach (var place in ownPlaces)
    //         {
    //             if (place.Type == c1.PlaceType)
    //                 samePlaceCount++;
    //         }
    //         return c0.SamePlaceCount.Output(samePlaceCount);
    //     }
    // }

    
}