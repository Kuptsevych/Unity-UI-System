using System;
using System.Collections.Generic;
using UISystem.Editor.Utils;
using UISystem.Runtime.Entities;
using UnityEditor;
using UnityEngine;

namespace UISystem.Runtime.Assets
{
    internal static class UIAssetsService
    {
        private static UIAssets _uiAssets;

        public static void CheckAsset(BaseView view)
        {
            if (!TryGetScreenType(view.GetType(), out var screenType))
            {
                Debug.LogError("Suitable screen type not found");
                return;
            }

            if (_uiAssets == null)
            {
                _uiAssets = Resources.Load<UIAssets>(Assets.UIAssetsAsset);
                if (_uiAssets == null)
                {
                    Debug.LogError("Create ui_assets asset before");
                    return;
                }

                var allViewsTypeAndAssetKey = GetAllViewsTypeAndAssetKey();

                foreach (var typeAndAssetKey in allViewsTypeAndAssetKey)
                {
                    _uiAssets.TryAddAsset(typeAndAssetKey.type, typeAndAssetKey.assetKey);
                }
            }

            var assetKey = EditorUtils.GetAddressFromAsset(view);
            if (assetKey != null)
            {
                _uiAssets.TryAddAsset(screenType, assetKey);
            }
        }

        private static bool TryGetScreenType(Type viewType, out Type screenType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var assemblyTypes = assembly.GetTypes();
                foreach (var assemblyType in assemblyTypes)
                {
                    if (assemblyType.IsSubclassOf(typeof(BaseScreen)))
                    {
                        var baseType = assemblyType.BaseType;
                        if (baseType != null)
                        {
                            var genericArguments = baseType.GetGenericArguments();
                            foreach (var type in genericArguments)
                            {
                                if (type == viewType)
                                {
                                    screenType = assemblyType;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            screenType = null;
            return false;
        }

        private static List<(Type type, string assetKey)> GetAllViewsTypeAndAssetKey()
        {
            var result = new List<(Type, string)>();

            var prefabs = GetAllPrefabs();
            foreach (var prefab in prefabs)
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(prefab);
                var view = go.GetComponent<BaseView>();

                if (view != null)
                {
                    if (TryGetScreenType(view.GetType(), out var screenType))
                    {
                        var assetKey = EditorUtils.GetAddressFromAsset(view);
                        if (assetKey != null)
                        {
                            result.Add((screenType, assetKey));
                        }
                    }
                }
            }

            return result;
        }

        private static string[] GetAllPrefabs()
        {
            var allAssetPaths = AssetDatabase.GetAllAssetPaths();
            var result = new List<string>();
            foreach (string assetPath in allAssetPaths)
            {
                if (assetPath.Contains(".prefab"))
                {
                    result.Add(assetPath);
                }
            }

            return result.ToArray();
        }
    }
}