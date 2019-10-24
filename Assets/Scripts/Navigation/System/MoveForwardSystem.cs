using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class MoveForwardSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref LocalToWorld d0, ref Translation d1, ref MotionData d2, ref MotionStatus d3) =>
        {
            if (d3.Status != NavigateStatus.Navigating) return;
            
            var distance = math.distance(d3.Position, d1.Value);

            // Why not go into branch?
            //Debug.Log(+distance);

            if (distance < d2.BreakDistance)
            {
                d3.Status = NavigateStatus.Arrived;
                //Debug.Log(d3.Status+""+distance);

                return;
            }

            var toSpeed = distance < d2.DecelerationDistance
            ? 0f 
            : d2.MaxSpeed;
            
            d2.Speed = math.lerp(d2.Speed, toSpeed, d2.LerpSpeed);
            d1.Value += d0.Forward * d2.Speed * Time.deltaTime;
        });
    }

    protected override void OnCreate() { }

    protected override void OnDestroy() { }
}

