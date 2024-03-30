using System.Collections.Generic;
using System.Linq;
using Entities;

namespace Core
{
	internal class ScreenStack
	{
		private readonly List<List<BaseScreen>> _screens = new();
		private readonly List<BaseScreen> _visible = new();
		private readonly HashSet<int> _nonBlockingLayers = new();
		private readonly Dictionary<BaseScreen, int> _screenLayers = new();

		private readonly int _layersCount;
		private readonly int _sortingOrderSpace;

		public ScreenStack(int layersCount, int sortingOrderSpace, IReadOnlyList<int> nonBlockingLayers)
		{
			_layersCount = layersCount;
			_sortingOrderSpace = sortingOrderSpace;
			for (int i = 0; i < layersCount; i++)
			{
				_screens.Add(new List<BaseScreen>());
			}

			for (var i = 0; i < nonBlockingLayers.Count; i++)
			{
				_nonBlockingLayers.Add(nonBlockingLayers[i]);
			}
		}

		public IReadOnlyList<BaseScreen> GetAllLayerScreens(int layer)
		{
			return _screens[layer].ToList();
		}

		public void Remove<TScreen>(TScreen screen) where TScreen : BaseScreen
		{
			_screenLayers.Remove(screen);
			Remove(screen, _screens, _visible, _nonBlockingLayers, _screenLayers, _layersCount, _sortingOrderSpace);
		}

		public void Add<TScreen>(TScreen screen, int layer = -1) where TScreen : BaseScreen
		{
			var screenLayer = layer < 0 ? screen.BaseScreenView.DefaultLayer : layer;
			_screenLayers.Add(screen, screenLayer);
			Add(screen, screenLayer, _screens, _visible, _nonBlockingLayers, _screenLayers, _layersCount, _sortingOrderSpace);
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

		public bool IsScreenInStack<TScreen>() where TScreen : BaseScreen
		{
			for (var i = 0; i < _screens.Count; i++)
			{
				var screensOnLayer = _screens[i];
				for (var n = 0; n < screensOnLayer.Count; n++)
				{
					if (screensOnLayer[n].GetType() == typeof(TScreen))
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool IsScreenOpened<TScreen>() where TScreen : BaseScreen
		{
			for (var i = 0; i < _visible.Count; i++)
			{
				if (_visible[i].GetType() == typeof(TScreen))
				{
					return true;
				}
			}

			return false;
		}

		private static void UpdateScreensOrder(List<BaseScreen> visible, int sortingOrderSpace)
		{
			var width = 0;
			var index = 0;
			for (var i = visible.Count - 1; i >= 0; i--)
			{
				var screen = visible[i];
				var screenWidth = screen.BaseScreenView.OrderWidth;
				var space = index * sortingOrderSpace;
				var order = width + space + screenWidth.maxDownShift;

				screen.BaseScreenView.SetSortingOrder(order);
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

					screen.BaseScreenView.Canvas.enabled = transparent;

					if (transparent && !screen.BaseScreenView.Transparent)
					{
						transparent = false;
					}
				}
			}
		}

		private static void UpdateScreensInput(List<BaseScreen> visible, HashSet<int> nonBlockingLayers, Dictionary<BaseScreen, int> screenLayers)
		{
			if (visible.Count > 0)
			{
				for (var i = visible.Count - 1; i >= 0; i--)
				{
					visible[i].BaseScreenView.Raycaster.enabled = false;
				}

				for (int i = 0; i < visible.Count; i++)
				{
					var screen = visible[i];

					var layer = screenLayers[screen];
					if (nonBlockingLayers.Contains(layer))
					{
						screen.BaseScreenView.Raycaster.enabled = true;
						continue;
					}

					screen.BaseScreenView.Raycaster.enabled = true;
					return;
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

		private static void Add<TScreen>(TScreen screen, int layer, List<List<BaseScreen>> screens, List<BaseScreen> visible, HashSet<int> nonBlockingLayers, Dictionary<BaseScreen, int> screenLayers,
			int layersCount, int sortingOrderSpace)
			where TScreen : BaseScreen
		{
			screens[layer].Add(screen);
			UpdateScreensVisibility(screens, visible, layersCount);
			UpdateScreensOrder(visible, sortingOrderSpace);
			UpdateScreensInput(visible, nonBlockingLayers, screenLayers);
		}

		private static void Remove<TScreen>(TScreen screen, List<List<BaseScreen>> screens, List<BaseScreen> visible, HashSet<int> nonBlockingLayers, Dictionary<BaseScreen, int> screenLayers,
			int layersCount, int sortingOrderSpace)
			where TScreen : BaseScreen
		{
			screens[screen.BaseScreenView.DefaultLayer].Remove(screen);
			UpdateScreensVisibility(screens, visible, layersCount);
			UpdateScreensOrder(visible, sortingOrderSpace);
			UpdateScreensInput(visible, nonBlockingLayers, screenLayers);
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