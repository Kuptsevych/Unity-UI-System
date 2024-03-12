using System;
using Assets;
using Entities;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
	public static class UI
	{
		private static UISettings _settings;
		private static UIAssets _uiAssets;
		private static ScreenManager _screenManager;

		[PublicAPI] public static bool Initialized { get; private set; }
		
		public static void Init()
		{
			if (Initialized)
			{
				return;
			}

			Initialized = true;

			_settings = LoadSettings();
			_uiAssets = LoadAndInitUIAssets();

			_screenManager = new ScreenManager(_settings, _uiAssets);
		}

		[PublicAPI]
		public static void SetCamera(Camera camera)
		{
			_screenManager.SetCamera(camera);
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

		public static bool IsScreenInStack<TScreen>() where TScreen : BaseScreen
		{
			return _screenManager.IsScreenInStack<TScreen>();
		}

		public static bool IsScreenOpened<TScreen>() where TScreen : BaseScreen
		{
			return _screenManager.IsScreenOpened<TScreen>();
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
			_screenManager.CloseAll();
		}

		public static void CloseAllOnLayer(int layer)
		{
			throw new NotImplementedException();
		}

		public static bool IsScreenTransparent()
		{
			throw new NotImplementedException();
		}

		public static int GetScreenLayer<TScreen>() where TScreen : BaseScreen
		{
			throw new NotImplementedException();
		}

		public static void SetTimeScale(int layer = -1)
		{
			throw new NotImplementedException();
		}

		public static int ScreensOnLayer(int layer)
		{
			return _screenManager.ScreensOnLayer(layer);
		}

		public static void Reset()
		{
			_screenManager.Reset();
		}

		internal static void Update(float deltaTime)
		{
			_screenManager.Update(deltaTime);
		}

		private static UISettings LoadSettings()
		{
			var settings = Resources.Load<UISettings>(Assets.UISystemAssets.SettingsAsset);
			if (settings == null)
			{
				throw new Exception("Create UI system settings before");
			}

			return settings;
		}

		private static UIAssets LoadAndInitUIAssets()
		{
			var uiAssets = Resources.Load<UIAssets>("ui_assets");
			if (uiAssets == null)
			{
				throw new Exception("Create ui_assets before");
			}

			uiAssets.Init();

			return uiAssets;
		}
	}
}