using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets
{
	[CreateAssetMenu(fileName = "ui_assets", menuName = "UI/UI Assets", order = 0)]
	public class UIAssets : ScriptableObject
	{
		[Serializable]
		private struct AssetTypeAndKey
		{
			public string TypeName;
			public string Key;
		}

		[SerializeField] private List<AssetTypeAndKey> _assetsList;

		private readonly Dictionary<Type, string> _assetsDictionary = new();

		public IReadOnlyDictionary<Type, string> AssetsDictionary => _assetsDictionary;

		internal void Init()
		{
			FillAssetsDictionary(_assetsDictionary, _assetsList);
		}

		private static void FillAssetsDictionary(Dictionary<Type, string> assetsDictionary, List<AssetTypeAndKey> assetsList)
		{
			assetsDictionary.Clear();
			foreach (var assetTypeAndKey in assetsList)
			{
				var type = Type.GetType(assetTypeAndKey.TypeName);
				if (type != null)
				{
					assetsDictionary.Add(type, assetTypeAndKey.Key);
				}
				else
				{
					Debug.LogError("Saved screen type error::" + assetTypeAndKey.TypeName);
				}
			}
		}

#if UNITY_EDITOR
		public void TryAddAsset(Type assetType, string assetKey)
		{
			if (System.Text.RegularExpressions.Regex.IsMatch(assetKey, @"\p{IsCyrillic}"))
			{
				throw new Exception($"Asset key for {assetType.Name} has cyrillic symbols");
			}

			if (_assetsDictionary.Count == 0)
			{
				FillAssetsDictionary(_assetsDictionary, _assetsList);
			}

			if (_assetsDictionary.TryGetValue(assetType, out var savedAssetKey))
			{
				if (savedAssetKey != assetKey)
				{
					_assetsDictionary[assetType] = assetKey;

					var typeAssemblyQualifiedName = assetType.AssemblyQualifiedName;
					var index = _assetsList.FindIndex(x => x.TypeName == typeAssemblyQualifiedName);

					_assetsList[index] = new AssetTypeAndKey
					{
						TypeName = assetType.AssemblyQualifiedName,
						Key = assetKey
					};

					EditorUtility.SetDirty(this);
					AssetDatabase.SaveAssets();
				}
			}
			else
			{
				_assetsDictionary.Add(assetType, assetKey);
				_assetsList.Add(new AssetTypeAndKey
				{
					TypeName = assetType.AssemblyQualifiedName,
					Key = assetKey
				});
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();
			}
		}

		[ContextMenu("Reset")]
		internal void Reset()
		{
			_assetsDictionary.Clear();
			_assetsList.Clear();
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}
#endif
	}
}