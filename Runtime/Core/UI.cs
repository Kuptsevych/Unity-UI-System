using System;
using UISystem.Runtime.Assets;
using UISystem.Runtime.Entities;
using UnityEngine;

namespace UISystem.Runtime.Core
{
    public static class UI
    {
        private static ScreenManager _screenManager;

        public static bool Initialized { get; private set; }

        private static UISettings _settings;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            if (Initialized)
            {
                return;
            }

            Initialized = true;

            _settings = LoadSettings();

            _screenManager = new ScreenManager(_settings);
        }

        public static TScreen Open<TScreen, TData>(TData data, bool instant)
            where TScreen : BaseScreen<TData>, new()
            where TData : struct
        {
            return _screenManager.Open<TScreen, TData>(data, instant);
        }

        public static void Close<TScreen>(TScreen screen, bool instant) where TScreen : BaseScreen
        {
            _screenManager.Close(screen, instant);
        }

        public static bool IsScreenInStack<TScreen>()
        {
            throw new NotImplementedException();
        }

        public static bool IsScreenOpened<TScreen>()
        {
            throw new NotImplementedException();
        }

        public static bool TryGetCurrentScreen(int layer, out BaseScreen screen)
        {
            return _screenManager.TryGetCurrentScreen(layer, out screen);
        }

        public static bool TryGetPreviousScreen(int layer, out BaseScreen screen)
        {
            return _screenManager.TryGetPreviousScreen(layer, out screen);
        }

        public static void CloseAll()
        {
            throw new NotImplementedException();
        }

        public static void CloseAllOnLayer(int layer)
        {
            throw new NotImplementedException();
        }

        public static bool IsScreenTransparent()
        {
            throw new NotImplementedException();
        }

        public static int GetScreenLayer<TScreen>()
        {
            throw new NotImplementedException();
        }

        public static void SetTimeScale(int layer = -1)
        {
        }

        public static int ScreensOnLayer(int layer)
        {
            return _screenManager.ScreensOnLayer(layer);
        }

        private static UISettings LoadSettings()
        {
            var settings = Resources.Load<UISettings>(Assets.Assets.SettingsAsset);
            if (settings == null)
            {
                throw new Exception("Create UI system settings before");
            }

            return settings;
        }
    }
}