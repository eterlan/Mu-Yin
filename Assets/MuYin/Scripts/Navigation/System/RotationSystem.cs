using MuYin.AI.Components.FSM;
using MuYin.Navigation.Component;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MuYin.Navigation.System
{
    [UpdateInGroup(typeof(NavigationSystemGroup))]
    public class RotationSystem : JobComponentSystem
    {
        [RequireComponentTag(typeof(InNavigation))]
        private struct Rotate : IJobForEachWithEntity<LocalToWorld, RotationEulerXYZ, MotionData, MotionInfo>
        {
            public float DeltaTime;
            public void Execute
            (
                Entity actor,
                int index,
                ref LocalToWorld     c0,
                ref RotationEulerXYZ c1,
                ref MotionData       c2,
                ref MotionInfo       c3)
            {
                var toTargetVector = c3.TargetPosition - c0.Position;
                toTargetVector.y = 0;
                var distance = math.length(toTargetVector);
                //Debug.Log($"targetPos: {target.Position}, localPos: {localToWorld.Position}, \ndistance: {distance}");
                if (distance < c2.DecelerationDistance)
                    return;
            
                var toTargetDir = math.normalizesafe(toTargetVector);
                var forward     = math.normalizesafe(c0.Forward);
                toTargetDir.y = 0f;
                forward.y     = 0f;
                var dirDiff    = math.distance(forward, toTargetDir);
                var toRotSpeed = dirDiff < 0.1 ? 0 : c2.MaxRotSpeed;
                //Debug.Log($"before: toRotSpeed: {toRotSpeed}, rotSpeed: {motion.RotSpeed}");
                c2.RotSpeed = math.lerp(c2.RotSpeed, toRotSpeed, c2.RotLerpSpeed);
                //Debug.Log($"after: toRotSpeed: {toRotSpeed}, rotSpeed: {motion.RotSpeed}");
                //Debug.Log($"forward: {forward}, toTargetVector: {toTargetDir}");
                if (dirDiff < 0.05f) 
                    return;
            
                var dtAngle = math.cross(forward, toTargetDir).y > 0 ? c2.RotSpeed : -c2.RotSpeed;
                //Debug.Log($"dirDiff: {dirDiff}");
                c1.Value.y += dtAngle * DeltaTime;
                //Debug.Log("dtTime"+Time.deltaTime+"dtAngle" + dtAngle * Time.deltaTime);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Rotate
            {
                DeltaTime = Time.deltaTime
            }.Schedule(this,inputDeps);
        }

        protected override void OnCreate() { }

        protected override void OnDestroy() { }
    }
}
