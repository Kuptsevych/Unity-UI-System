using System.Linq;
using Assets;
using Entities;
using UnityEditor;
using UnityEngine;

namespace RJ.UISystem.Editor.Inspector
{
    [CustomPropertyDrawer(typeof(BaseScreenView.BaseScreenViewSettings))]
    public class ScreenViewSettingsPropertyDrawer : PropertyDrawer
    {
        private static UISettings _settings;
        private static string[] _layers;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_settings == null)
            {
                _settings = Resources.Load<UISettings>("ui_settings");

                if (_settings == null)
                {
                    Debug.LogError("Fail to load UI settings asset");
                    return;
                }

                _layers = _settings.Layers.ToArray();
            }

            EditorGUI.BeginProperty(position, label, property);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var boxRect = new Rect(position.x, position.y, position.width, position.height - 5);
            var layerPopupRect = new Rect(position.x, position.y + 30, position.width, 18);
            var transparentRect = new Rect(position.x, position.y + 50, position.width, 18);
            var openTransitionRect = new Rect(position.x, position.y + 70, position.width, 18);
            var showTransitionRect = new Rect(position.x, position.y + 90, position.width, 18);
            var hideTransitionRect = new Rect(position.x, position.y + 110, position.width, 18);
            var closeTransitionRect = new Rect(position.x, position.y + 130, position.width, 18);
            var spareUpRect = new Rect(position.x, position.y + 150, position.width * 0.5f, 18);
            var spareDownRect = new Rect(position.x + position.width * 0.51f, position.y + 150, position.width * 0.5f, 18);
            var endRect = new Rect(position.x, position.y + 175, position.width, 1);

            GUI.Box(boxRect, "Screen view settings");

            var serializedProperty = property.FindPropertyRelative("_defaultLayer");
            serializedProperty.intValue = EditorGUI.Popup(EditorGUI.PrefixLabel(layerPopupRect, new GUIContent("Default layer")), serializedProperty.intValue, _layers);

            EditorGUI.PropertyField(EditorGUI.PrefixLabel(transparentRect, new GUIContent("Is transparent")), property.FindPropertyRelative("_transparent"), GUIContent.none);
            EditorGUI.PropertyField(EditorGUI.PrefixLabel(openTransitionRect, new GUIContent("Prefer open transition")), property.FindPropertyRelative("_preferOpenTransition"), GUIContent.none);
            EditorGUI.PropertyField(EditorGUI.PrefixLabel(showTransitionRect, new GUIContent("Prefer show transition")), property.FindPropertyRelative("_preferShowTransition"), GUIContent.none);
            EditorGUI.PropertyField(EditorGUI.PrefixLabel(hideTransitionRect, new GUIContent("Prefer hide transition")), property.FindPropertyRelative("_preferHideTransition"), GUIContent.none);
            EditorGUI.PropertyField(EditorGUI.PrefixLabel(closeTransitionRect, new GUIContent("Prefer close transition")), property.FindPropertyRelative("_preferCloseTransition"), GUIContent.none);

            EditorGUI.PropertyField(EditorGUI.PrefixLabel(spareUpRect, new GUIContent("Spare up width")), property.FindPropertyRelative("_spareUpWidth"), GUIContent.none);
            EditorGUI.PropertyField(EditorGUI.PrefixLabel(spareDownRect, new GUIContent("Spare down width")), property.FindPropertyRelative("_spareDownWidth"), GUIContent.none);

            EditorGUI.DrawRect(endRect, new Color(0.5f, 0.5f, 0.5f, 1));

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 180;
        }
    }
}