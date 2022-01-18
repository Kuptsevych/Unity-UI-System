using UISystem.Runtime.Assets;
using UISystem.Runtime.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace UISystem.Runtime.Core
{
    internal class ScreenManager
    {
        private readonly UISettings _settings;
        private readonly GameObject _uiRoot;
        private readonly Camera _uiCamera;
        private readonly EventSystem _eventSystem;

        private readonly ScreenPool _screenPool;
        private readonly ScreenStack _screenStack;
        private readonly ScreensTransition _screensTransition;

        public ScreenManager(UISettings settings)
        {
            _settings = settings;
            _uiRoot = CreateRoot(_settings, this);
            _uiCamera = CreateCamera(_uiRoot, _settings);
            _eventSystem = CreateEventSystem(_uiRoot);
            _screenStack = new ScreenStack(_settings.LayersCount, _settings.SortingOrderSpace);
            _screensTransition = CreateScreensTransition(_settings);
            _screenPool = new ScreenPool(_uiRoot.transform, _uiCamera);
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

        private static GameObject CreateRoot(UISettings settings, ScreenManager screenManager)
        {
            var root = new GameObject(settings.RootName);
            root.AddComponent<Root>().Connect(screenManager);
            Object.DontDestroyOnLoad(root);
            return root;
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
            //TODO move to settings
            return camera;
        }

        private static EventSystem CreateEventSystem(GameObject root)
        {
            var eventSystem = Object.FindObjectOfType<EventSystem>();

            if (eventSystem == null)
            {
                var eventSystemHolder = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                eventSystemHolder.transform.SetParent(root.transform);
                eventSystem = eventSystemHolder.GetComponent<EventSystem>();
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

            screenStack.TryGetCurrent(screen.BaseView.DefaultLayer, out var currentScreen);

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
                screenStack.TryGetPrevious(screen.BaseView.DefaultLayer, out var previousScreen);

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