using System.Collections.Generic;
using UISystem.Runtime.Entities;

namespace UISystem.Runtime.Core
{
    internal class ScreenStack
    {
        private readonly List<List<BaseScreen>> _screens = new();
        private readonly List<BaseScreen> _visible = new();

        private readonly int _layersCount;
        private readonly int _sortingOrderSpace;

        public ScreenStack(int layersCount, int sortingOrderSpace)
        {
            _layersCount = layersCount;
            _sortingOrderSpace = sortingOrderSpace;
            for (int i = 0; i < layersCount; i++)
            {
                _screens.Add(new List<BaseScreen>());
            }
        }

        public void Remove<TScreen>(TScreen screen) where TScreen : BaseScreen
        {
            Remove(screen, _screens, _visible, _layersCount, _sortingOrderSpace);
        }

        public void Add<TScreen>(TScreen screen) where TScreen : BaseScreen
        {
            Add(screen, _screens, _visible, _layersCount, _sortingOrderSpace);
        }

        public bool TryGetCurrent(int layer, out BaseScreen screen)
        {
            return TryGetCurrent(_screens, layer, out screen);
        }

        public bool TryGetPrevious(int layer, out BaseScreen screen)
        {
            return TryGetPrevious(_screens, layer, out screen);
        }

        public void Update(float deltaTime)
        {
            for (var i = _visible.Count - 1; i >= 0; i--)
            {
                _visible[i].Update(deltaTime);
            }
        }

        public int ScreensOnLayer(int layer)
        {
            return _screens[layer].Count;
        }

        private static void UpdateScreensOrder(List<BaseScreen> visible, int sortingOrderSpace)
        {
            var width = 0;
            var index = 0;
            for (var i = visible.Count - 1; i >= 0; i--)
            {
                var screen = visible[i];
                var screenWidth = screen.BaseView.OrderWidth;
                var space = index * sortingOrderSpace;
                var order = width + space + screenWidth.maxDownShift;

                screen.BaseView.SetSortingOrder(order);
                width += screenWidth.maxUpShift;
                index++;
            }
        }

        private static void UpdateScreensVisibility(List<List<BaseScreen>> screens, List<BaseScreen> visible, int layersCount)
        {
            visible.Clear();

            var transparent = true;

            for (int i = layersCount - 1; i >= 0; i--)
            {
                var layerScreens = screens[i];

                for (var n = layerScreens.Count - 1; n >= 0; n--)
                {
                    var screen = layerScreens[n];

                    if (transparent)
                    {
                        visible.Add(screen);
                    }

                    screen.BaseView.Canvas.enabled = transparent;

                    if (transparent && !screen.BaseView.Transparent)
                    {
                        transparent = false;
                    }
                }
            }
        }

        private static bool HasScreens(List<List<BaseScreen>> screens, int layer)
        {
            return screens[layer].Count > 0;
        }

        private static bool HasPreviousScreen(List<List<BaseScreen>> screens, int layer)
        {
            return screens[layer].Count > 1;
        }

        private static void Add<TScreen>(TScreen screen, List<List<BaseScreen>> screens, List<BaseScreen> visible, int layersCount, int sortingOrderSpace)
            where TScreen : BaseScreen
        {
            screens[screen.BaseView.DefaultLayer].Add(screen);
            UpdateScreensVisibility(screens, visible, layersCount);
            UpdateScreensOrder(visible, sortingOrderSpace);
        }

        private static void Remove<TScreen>(TScreen screen, List<List<BaseScreen>> screens, List<BaseScreen> visible, int layersCount, int sortingOrderSpace)
            where TScreen : BaseScreen
        {
            screens[screen.BaseView.DefaultLayer].Remove(screen);
            UpdateScreensVisibility(screens, visible, layersCount);
            UpdateScreensOrder(visible, sortingOrderSpace);
        }

        private static bool TryGetCurrent(List<List<BaseScreen>> screens, int layer, out BaseScreen screen)
        {
            if (HasScreens(screens, layer))
            {
                screen = screens[layer][^1];
                return true;
            }

            screen = null;
            return false;
        }

        private static bool TryGetPrevious(List<List<BaseScreen>> screens, int layer, out BaseScreen screen)
        {
            if (HasPreviousScreen(screens, layer))
            {
                screen = screens[layer][^2];
                return true;
            }

            screen = null;
            return false;
        }
    }
}