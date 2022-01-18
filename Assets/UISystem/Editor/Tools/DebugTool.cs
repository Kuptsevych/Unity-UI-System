using UnityEditor;
using UnityEngine;

namespace UISystem.Editor.Tools
{
    public class DebugTool : EditorWindow
    {
        [MenuItem("UI/Debug")]
        private static void ShowWindow()
        {
            var window = GetWindow<DebugTool>();
            window.titleContent = new GUIContent("UI Debug");
            window.Show();
        }

        private void OnGUI()
        {
        }
    }
}