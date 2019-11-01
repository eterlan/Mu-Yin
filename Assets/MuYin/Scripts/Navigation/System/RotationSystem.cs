using MuYin.Navigation.Component;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MuYin.Navigation.System
{
    public class RotationSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref LocalToWorld localToWorld, ref RotationEulerXYZ rotation, ref MotionData motion, ref 
                                  MotionInfo 
                                  target) =>
            {
                if (target.NavigateStatus != NavigateStatus.Navigating) return;
            
                var toTargetVector = target.TargetPosition - localToWorld.Position;
                toTargetVector.y = 0;
                var distance = math.length(toTargetVector);
                //Debug.Log($"targetPos: {target.Position}, localPos: {localToWorld.Position}, \ndistance: {distance}");
                if (distance < motion.DecelerationDistance)
                    return;
            
                var toTargetDir = math.normalizesafe(toTargetVector);
                var forward     = math.normalizesafe(localToWorld.Forward);
                toTargetDir.y = 0f;
                forward.y     = 0f;
                var dirDiff    = math.distance(forward, toTargetDir);
                var toRotSpeed = dirDiff < 0.1 ? 0 : motion.MaxRotSpeed;
                //Debug.Log($"before: toRotSpeed: {toRotSpeed}, rotSpeed: {motion.RotSpeed}");
                motion.RotSpeed = math.lerp(motion.RotSpeed, toRotSpeed, motion.RotLerpSpeed);
                //Debug.Log($"after: toRotSpeed: {toRotSpeed}, rotSpeed: {motion.RotSpeed}");
                //Debug.Log($"forward: {forward}, toTargetVector: {toTargetDir}");
                if (dirDiff < 0.05f) 
                    return;
            
                var dtAngle = math.cross(forward, toTargetDir).y > 0 ? motion.RotSpeed : -motion.RotSpeed;
                //Debug.Log($"dirDiff: {dirDiff}");
                rotation.Value.y += dtAngle * Time.deltaTime;
                //Debug.Log("dtTime"+Time.deltaTime+"dtAngle" + dtAngle * Time.deltaTime);
            });
        }

        protected override void OnCreate() { }

        protected override void OnDestroy() { }
    }
}
