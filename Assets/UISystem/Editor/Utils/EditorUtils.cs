using UnityEditor;
using UnityEngine;

namespace UISystem.Editor.Utils
{
    public static class EditorUtils
    {
        public static string GetAddressFromAsset(Object asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            var guid = AssetDatabase.AssetPathToGUID(path);
            var assetEntry = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid);
            return assetEntry?.address;
        }
    }
}