using System;
using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Assets
{
#if UNITY_EDITOR
	internal static class UIAssetsService
	{
		private static UIAssets _uiAssets;

		[UnityEditor.MenuItem("UI/Update UIAssets")]
		public static void UpdateUIAssets()
		{
			_uiAssets = Resources.Load<UIAssets>(UISystemAssets.UIAssetsAsset);
			if (_uiAssets == null)
			{
				Debug.LogError("Create ui_assets asset before");
				return;
			}

			_uiAssets.Reset();

			var allViewsTypeAndAssetKey = GetAllViewsTypeAndAssetKey();

			foreach (var typeAndAssetKey in allViewsTypeAndAssetKey)
			{
				_uiAssets.TryAddAsset(typeAndAssetKey.type, typeAndAssetKey.assetKey);
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
				var go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefab);
				if (go != null)
				{
					var view = go.GetComponent<BaseView>();

					if (view != null)
					{
						if (TryGetScreenType(view.GetType(), out var screenType))
						{
							var assetKey = UISystem.Editor.Utils.EditorUtils.GetAddressFromAsset(view);
							if (assetKey != null)
							{
								result.Add((screenType, assetKey));
							}
						}
					}
				}
			}

			return result;
		}

		private static string[] GetAllPrefabs()
		{
			var allAssetPaths = UnityEditor.AssetDatabase.GetAllAssetPaths();
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
#endif
}