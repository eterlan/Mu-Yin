using MuYin.AI.Systems;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using MuYin.AI.Components;

namespace MuYin.AI.Editor
{
    [CustomEditor(typeof(FieldOfViewAuthoring))]
    public class FieldOfViewAuthoringEditor : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            var vd = (FieldOfViewAuthoring) target;
            Handles.color = Color.white;
            var transform = vd.transform;
            var position = (float3)transform.position;
            Handles.DrawWireArc(position, transform.up, transform.forward, 360, vd.Radius);
            Handles.DrawLine(position, position + vd.Deg2Dir(-vd.Angle/2, false) * vd.Radius);
            Handles.DrawLine(position, position + vd.Deg2Dir(vd.Angle/2, false) * vd.Radius);
        }
    }
}
