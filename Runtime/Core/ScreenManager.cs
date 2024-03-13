using Assets;
using Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Core
{
	internal class ScreenManager
	{
		private readonly UISettings _settings;
		private readonly GameObject _uiRoot;
		private readonly EventSystem _eventSystem;

		private readonly ScreenPool _screenPool;
		private readonly ScreenStack _screenStack;
		private readonly ScreensTransition _screensTransition;

		private Camera _uiCamera;

		public ScreenManager(UISettings settings, UIAssets uiAssets)
		{
			_settings = settings;
			_uiRoot = CreateRoot(_settings, ref _uiCamera);
			_eventSystem = CreateEventSystem(_uiRoot);
			_screenStack = new ScreenStack(_settings.LayersCount, _settings.SortingOrderSpace, _settings.NonBlockingLayers);
			_screensTransition = CreateScreensTransition(_settings);
			_screenPool = new ScreenPool(_uiRoot.transform, _uiCamera, uiAssets);
		}

		public void SetCamera(Camera camera)
		{
			_uiCamera = camera;
			UpdateRenderMode(_uiRoot, _uiCamera);
		}

		public void Update(float deltaTime)
		{
			_screenStack.Update(deltaTime);
			_screensTransition.Update(deltaTime);
		}

		public TScreen Open<TScreen, TData>(TData data, bool instant)
			where TScreen : BaseScreen<TData>, new()
			where TData : struct
		{
			return Open<TScreen, TData>(data, instant, _screenPool, _screenStack, _screensTransition);
		}

		public void Close<TScreen>(TScreen screen, bool instant) where TScreen : BaseScreen
		{
			Close(screen, instant, _screenPool, _screenStack, _screensTransition);
		}

		public bool TryGetCurrentScreen(int layer, out BaseScreen screen)
		{
			return _screenStack.TryGetCurrent(layer, out screen);
		}

		public bool TryGetPreviousScreen(int layer, out BaseScreen screen)
		{
			return _screenStack.TryGetPrevious(layer, out screen);
		}

		public int ScreensOnLayer(int layer)
		{
			return _screenStack.ScreensOnLayer(layer);
		}

		public void CloseAll()
		{
			for (int i = 0; i < _settings.LayersCount; i++)
			{
				var layerScreens = _screenStack.GetAllLayerScreens(i);
				for (var n = 0; n < layerScreens.Count; n++)
				{
					Close(layerScreens[n], true);
				}
			}
		}

		public void Reset()
		{
			_screensTransition.TryForceComplete();
			CloseAll();
			_screenPool.Reset();
		}

		public TScreen CreateContainerScreen<TScreen, TScreenView>(int layer)
			where TScreen : BaseScreen, new()
			where TScreenView : BaseScreenView
		{
			var screen = _screenPool.CreateContainerScreen<TScreen, TScreenView>();
			_screenStack.Add(screen, layer);
			return screen;
		}

		public bool IsScreenInStack<TScreen>() where TScreen : BaseScreen
		{
			return _screenStack.IsScreenInStack<TScreen>();
		}

		public bool IsScreenOpened<TScreen>() where TScreen : BaseScreen
		{
			return _screenStack.IsScreenOpened<TScreen>();
		}

		private static GameObject CreateRoot(UISettings settings, ref Camera uiCamera)
		{
			var root = new GameObject(settings.RootName);
			root.AddComponent<Root>();
			root.AddComponent<Canvas>();

			var scaler = root.AddComponent<CanvasScaler>();

			if (settings.CreateCamera)
			{
				uiCamera = CreateCamera(root, settings);
			}

			UpdateRenderMode(root, uiCamera);

			scaler.uiScaleMode = settings.ScaleMode;
			scaler.screenMatchMode = settings.ScreenMatchMode;
			scaler.matchWidthOrHeight = settings.MatchWidthOrHeight;
			scaler.referenceResolution = settings.ReferenceResolution;

			Object.DontDestroyOnLoad(root);
			return root;
		}

		private static void UpdateRenderMode(GameObject root, Camera uiCamera)
		{
			var canvas = root.GetComponent<Canvas>();

			if (uiCamera != null)
			{
				canvas.renderMode = RenderMode.ScreenSpaceCamera;
				canvas.worldCamera = uiCamera;
			}
			else
			{
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			}
		}

		private static Camera CreateCamera(GameObject root, UISettings settings)
		{
			var camera = root.AddComponent<Camera>();
			camera.clearFlags = CameraClearFlags.Depth;
			camera.cullingMask = settings.CullingMask;
			camera.orthographic = true;
			camera.allowMSAA = false;
			camera.allowHDR = false;
			camera.useOcclusionCulling = false;
			camera.allowDynamicResolution = false;
			camera.depth = settings.CameraDepth;

			var cameraData = camera.GetUniversalAdditionalCameraData();
			cameraData.renderType = CameraRenderType.Overlay;

			//TODO move to settings
			return camera;
		}

		private static EventSystem CreateEventSystem(GameObject root)
		{
			var eventSystem = Object.FindFirstObjectByType<EventSystem>();

			if (eventSystem == null)
			{
				var eventSystemHolder = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
				eventSystemHolder.transform.SetParent(root.transform);
				eventSystem = eventSystemHolder.GetComponent<EventSystem>();
				Object.DontDestroyOnLoad(eventSystemHolder);
			}

			return eventSystem;
		}

		private static ScreensTransition CreateScreensTransition(UISettings settings)
		{
			var definedTransitions = settings.GetDefinedTransitions();
			return new ScreensTransition(new ScreensTransitionData(definedTransitions, settings.TransitionTypesOrder));
		}

		private static TScreen Open<TScreen, TData>(TData data, bool instant, ScreenPool screenPool, ScreenStack screenStack, ScreensTransition screensTransition)
			where TScreen : BaseScreen<TData>, new()
			where TData : struct
		{
			var screen = screenPool.GetScreen<TScreen>();

			var screenWithData = (BaseScreen<TData>) screen;

			screenWithData.SetData(data);

			screenStack.TryGetCurrent(screen.BaseScreenView.DefaultLayer, out var currentScreen);

			screenStack.Add(screen);

			screensTransition.StartOpenTransition(currentScreen, screen, instant);

			return screen;
		}

		private static void Close<TScreen>(TScreen screen, bool instant, ScreenPool screenPool, ScreenStack screenStack, ScreensTransition screensTransition)
			where TScreen : BaseScreen
		{
			var isCurrentScreen = screen.State == ScreenState.Opened;
			if (isCurrentScreen)
			{
				screenStack.TryGetPrevious(screen.BaseScreenView.DefaultLayer, out var previousScreen);

				screensTransition.StartCloseTransition(screen, previousScreen, instant, () =>
				{
					screenStack.Remove(screen);
					screenPool.ReleaseScreen(screen);
				});
			}
			else
			{
				screen.Close(true);
				screenStack.Remove(screen);
				screenPool.ReleaseScreen(screen);
			}
		}
	}
}