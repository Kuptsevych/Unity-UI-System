using System;
using System.Collections.Generic;
using System.Linq;
using Assets;
using Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Core
{
	internal sealed class ScreenPool
	{
		private readonly Dictionary<Type, (HashSet<BaseScreen> free, HashSet<BaseScreen> taken)> _screenPool = new();

		private readonly List<BaseScreen> _containerScreens = new();

		private readonly Transform _root;
		private readonly Camera _uiCamera;
		private readonly UIAssets _uiAssets;

		public ScreenPool(Transform root, Camera uiCamera, UIAssets uiAssets)
		{
			_root = root;
			_uiCamera = uiCamera;
			_uiAssets = uiAssets;
		}

		public TScreen GetScreen<TScreen>() where TScreen : BaseScreen, new()
		{
			return GetScreen<TScreen>(_screenPool, _root, _uiCamera, _uiAssets);
		}

		public void ReleaseScreen<TScreen>(TScreen screen) where TScreen : BaseScreen
		{
			ReleaseScreen(screen, _screenPool);
		}

		private static TScreen CreateScreen<TScreen>(Transform root, Camera camera, UIAssets uiAssets) where TScreen : BaseScreen, new()
		{
			var screen = new TScreen();

			var screenPrefab = LoadScreenPrefab(typeof(TScreen), uiAssets);

			if (screenPrefab != null)
			{
				var screenViewInstance = UnityEngine.Object.Instantiate(screenPrefab, root);

				var screenView = screenViewInstance.GetComponent<BaseScreenView>();

				if (screenView != null)
				{
					screenView.Init();

					screenView.Canvas.overrideSorting = true;
					screenView.Canvas.enabled = false;

					screen.ConnectView(screenView);

					return screen;
				}

				throw new Exception($"Screen view not found ({typeof(TScreen)})");
			}

			throw new Exception($"Screen prefab loading failed, for type({typeof(TScreen)}");
		}

		private static GameObject LoadScreenPrefab(Type type, UIAssets uiAssets)
		{
			if (uiAssets.AssetsDictionary.TryGetValue(type, out var assetKey))
			{
				var asyncLoadingAssetHandle = Addressables.LoadAssetAsync<GameObject>(assetKey);
				return asyncLoadingAssetHandle.WaitForCompletion();
				//TODO temporary solution for synchronous loading
			}

			throw new Exception($"Asset key not found for type ({type})");
		}

		private static void ReleaseScreen<TScreen>(TScreen screen, IReadOnlyDictionary<Type, (HashSet<BaseScreen> free, HashSet<BaseScreen> taken)> screenPool)
			where TScreen : BaseScreen
		{
			if (screenPool.TryGetValue(typeof(TScreen), out var screens))
			{
				screens.taken.Remove(screen);
				screens.free.Add(screen);
			}
			else
			{
				throw new Exception("Try to release unknown type screen");
			}
		}

		private static TScreen GetScreen<TScreen>(Dictionary<Type, (HashSet<BaseScreen> free, HashSet<BaseScreen> taken)> screenPool, Transform root, Camera uiCamera,
			UIAssets uiAssets)
			where TScreen : BaseScreen, new()
		{
			if (screenPool.TryGetValue(typeof(TScreen), out var screens))
			{
				if (screens.free.Count > 0)
				{
					var screenFromPool = screens.free.First();
					screens.free.Remove(screenFromPool);
					screens.taken.Add(screenFromPool);
					return (TScreen) screenFromPool;
				}
			}

			var screen = CreateScreen<TScreen>(root, uiCamera, uiAssets);
			if (screens.taken != null)
			{
				screens.taken.Add(screen);
			}
			else
			{
				screenPool.Add(typeof(TScreen), (new HashSet<BaseScreen>(), new HashSet<BaseScreen>
				{
					screen
				}));
			}

			return screen;
		}

		public void Reset()
		{
			foreach (var (_, (free, taken)) in _screenPool)
			{
				Debug.Assert(taken.Count == 0);

				foreach (var screen in free)
				{
					screen.Utilize();
				}
			}

			_screenPool.Clear();
			_containerScreens.Clear();
		}

		public TScreen CreateContainerScreen<TScreen, TScreenView>()
			where TScreen : BaseScreen, new()
			where TScreenView : BaseScreenView
		{
			var screenGameObject = new GameObject(typeof(TScreen).Name);
			screenGameObject.transform.SetParent(_root);
			screenGameObject.AddComponent<Canvas>();
			screenGameObject.AddComponent<GraphicRaycaster>();
			var screenView = screenGameObject.AddComponent<TScreenView>();
			screenView.Init();
			var screen = new TScreen();
			screen.ConnectView(screenView);
			_containerScreens.Add(screen);

			return screen;
		}
	}
}