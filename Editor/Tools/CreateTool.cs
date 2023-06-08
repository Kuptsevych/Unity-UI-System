using UnityEditor;
using UnityEngine;

namespace UISystem.Editor.Tools
{
    public class CreateTool : EditorWindow
    {
        [MenuItem("UI/Create")]
        private static void ShowWindow()
        {
            var window = GetWindow<CreateTool>();
            window.titleContent = new GUIContent("UI Create");
            window.Show();
        }

        private void OnGUI()
        {
        }
    }
}