using UnityEditor;
using UnityEngine;

namespace AI.Consideration
{
    [CustomPropertyDrawer(typeof(SleepConsiderer))]
    public class SleepConsidererDrawer : PropertyDrawer
    {
        public override void OnGUI
        (
            Rect               position,
            SerializedProperty property,
            GUIContent         label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property.FindPropertyRelative("m_score"));
            EditorGUI.EndProperty();
        }
    }
}