using System;
using System.Collections.Generic;
using System.Linq;
using UISystem.Runtime.Assets;
using UISystem.Runtime.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UISystem.Runtime.Core
{
    internal sealed class ScreenPool
    {
        private readonly Dictionary<Type, (HashSet<BaseScreen> free, HashSet<BaseScreen> taken)> _screenPool = new();

        private readonly Transform _root;
        private readonly Camera _uiCamera;
        private readonly UIAssets _uiAssets;

        public ScreenPool(Transform root, Camera uiCamera)
        {
            _root = root;
            _uiCamera = uiCamera;
            _uiAssets = PrepareUIAsset();
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

                var screenView = screenViewInstance.GetComponent<BaseView>();

                if (screenView != null)
                {
                    screenView.Init();
                    screenView.Canvas.worldCamera = camera;
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
            if (uiAssets.TryGetAssetKey(type, out var assetKey))
            {
                var asyncLoadingAssetHandle = Addressables.LoadAssetAsync<GameObject>(assetKey);
                return asyncLoadingAssetHandle.WaitForCompletion();
                //TODO temporary solution for synchronous loading
            }

            throw new Exception($"Asset key not found for type ({type})");
        }

        private static UIAssets PrepareUIAsset()
        {
            var uiAssets = Resources.Load<UIAssets>("ui_assets");
            if (uiAssets == null)
            {
                throw new Exception("Create ui_assets before");
            }

            uiAssets.Init();

            return uiAssets;
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

        private static TScreen GetScreen<TScreen>(Dictionary<Type, (HashSet<BaseScreen> free, HashSet<BaseScreen> taken)> screenPool, Transform root, Camera uiCamera, UIAssets uiAssets)
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
                screenPool.Add(typeof(TScreen), (new HashSet<BaseScreen>(), new HashSet<BaseScreen> {screen}));
            }

            return screen;
        }
    }
}