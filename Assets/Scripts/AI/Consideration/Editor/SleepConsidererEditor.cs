using System;
using UnityEditor;

namespace AI.Consideration
{
    [CustomEditor(typeof(SleepConsiderer))]
    [CanEditMultipleObjects]
    public class SleepConsidererEditor : Editor
    {
        SerializedProperty m_score;

        private void OnEnable() { m_score = serializedObject.FindProperty("m_score"); }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_score);
            serializedObject.ApplyModifiedProperties();
        }
    }
}