using System;
using System.Collections.Generic;
using Components;
using Core;

namespace Entities
{
	public abstract class BaseScreen
	{
		public event Action<(BaseScreen screen, ScreenState prevState, ScreenState newState)> OnStateChanged;

		private ScreenState _state;

		private readonly List<BaseComponent<BaseScreen>> _components = new();

		internal BaseScreenView BaseScreenView { get; private set; }

		public ScreenState State
		{
			get => _state;
			private set => SwitchState(this, value);
		}

		internal void Update(float deltaTime)
		{
			OnUpdate(deltaTime);
			BaseScreenView.UpdateView(deltaTime);
			UpdateComponents(deltaTime);
		}

		internal void ConnectView(BaseScreenView baseScreenView)
		{
			BaseScreenView = baseScreenView;
			OnConnectView();
			PrepareComponents(baseScreenView);
		}

		internal abstract void OnConnectView();

		internal ScreenSwitchStateHandler Open(bool instant = false)
		{
			return Open(this, instant);
		}

		internal ScreenSwitchStateHandler Show(bool instant = false)
		{
			return Show(this, instant);
		}

		internal ScreenSwitchStateHandler Hide(bool instant = false)
		{
			return Hide(this, instant);
		}

		internal ScreenSwitchStateHandler Close(bool instant = false)
		{
			return Close(this, instant);
		}

		internal void Utilize()
		{
			foreach (var component in _components)
			{
				component.CleanUp();
			}

			UnityEngine.Object.Destroy(BaseScreenView.gameObject);
		}

		protected virtual void OnUpdate(float deltaTime)
		{
		}

		protected virtual void OnOpen()
		{
		}

		protected virtual void OnOpening()
		{
		}

		protected virtual void OnClose()
		{
		}

		protected virtual void OnClosing()
		{
		}

		protected virtual void OnShow()
		{
		}

		protected virtual void OnShowing()
		{
		}

		protected virtual void OnHide()
		{
		}

		protected virtual void OnHiding()
		{
		}

		internal void SetSwitchStateProgress(float normalizedTime)
		{
			SetSwitchStateProgress(this, normalizedTime);
		}

		internal void CompleteSwitchState()
		{
			CompleteSwitchState(this);
		}

		private void PrepareComponents(BaseScreenView baseScreenView)
		{
			_components.Clear();
			var components = baseScreenView.GetComponents<BaseComponent<BaseScreen>>();
			if (components.Length > 0)
			{
				for (var i = 0; i < _components.Count; i++)
				{
					_components[i].Init(this);
				}

				for (var i = 0; i < _components.Count; i++)
				{
					_components[i].Ready();
				}

				_components.AddRange(components);
			}
		}

		private void UpdateComponents(float deltaTime)
		{
			for (var i = 0; i < _components.Count; i++)
			{
				_components[i].UpdateComponent(deltaTime);
			}
		}

		private static ScreenSwitchStateHandler Open(BaseScreen screen, bool instant)
		{
			if (instant || screen.BaseScreenView.CloseDuration <= 0)
			{
				screen.State = ScreenState.Opened;
				screen.OnOpen();
				screen.BaseScreenView.Canvas.enabled = true;
				return default;
			}

			screen.State = ScreenState.Opening;
			screen.OnOpening();
			return new ScreenSwitchStateHandler(screen, screen.BaseScreenView.OpenDuration, screen.State);
		}

		private static ScreenSwitchStateHandler Show(BaseScreen screen, bool instant)
		{
			if (instant || screen.BaseScreenView.CloseDuration <= 0)
			{
				screen.State = ScreenState.Opened;
				screen.OnShow();
				return default;
			}

			screen.State = ScreenState.Showing;
			screen.OnShowing();
			return new ScreenSwitchStateHandler(screen, screen.BaseScreenView.ShowDuration, screen.State);
		}

		private static ScreenSwitchStateHandler Hide(BaseScreen screen, bool instant)
		{
			if (instant || screen.BaseScreenView.CloseDuration <= 0)
			{
				screen.State = ScreenState.Hided;
				screen.OnHide();
				return default;
			}

			screen.State = ScreenState.Hiding;
			screen.OnHiding();
			return new ScreenSwitchStateHandler(screen, screen.BaseScreenView.HideDuration, screen.State);
		}

		private static ScreenSwitchStateHandler Close(BaseScreen screen, bool instant)
		{
			if (instant || screen.BaseScreenView.CloseDuration <= 0)
			{
				screen.State = ScreenState.Closed;
				screen.BaseScreenView.Canvas.enabled = false;
				screen.OnClose();
				return default;
			}

			screen.State = ScreenState.Closing;
			screen.OnClosing();
			return new ScreenSwitchStateHandler(screen, screen.BaseScreenView.CloseDuration, screen.State);
		}

		private static void SwitchState(BaseScreen screen, ScreenState state)
		{
			if (state != screen._state)
			{
				var prevState = screen._state;
				screen._state = state;
				screen.OnStateChanged?.Invoke((screen, prevState, state));
			}
		}

		private static void SetSwitchStateProgress(BaseScreen screen, float normalizedTime)
		{
			switch (screen.State)
			{
				case ScreenState.Opening:
					screen.BaseScreenView.SetOpeningProgress(normalizedTime);
					break;
				case ScreenState.Showing:
					screen.BaseScreenView.SetShowingProgress(normalizedTime);
					break;
				case ScreenState.Hiding:
					screen.BaseScreenView.SetHidingProgress(normalizedTime);
					break;
				case ScreenState.Closing:
					screen.BaseScreenView.SetClosingProgress(normalizedTime);
					break;
			}
		}

		private static void CompleteSwitchState(BaseScreen screen)
		{
			switch (screen.State)
			{
				case ScreenState.Opening:
					screen.State = ScreenState.Opened;
					screen.BaseScreenView.Canvas.enabled = true;
					screen.OnOpen();
					break;
				case ScreenState.Showing:
					screen.State = ScreenState.Opened;
					screen.OnShow();
					break;
				case ScreenState.Hiding:
					screen.State = ScreenState.Hided;
					screen.OnHide();
					break;
				case ScreenState.Closing:
					screen.State = ScreenState.Closed;
					screen.BaseScreenView.Canvas.enabled = false;
					screen.OnClose();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public abstract class BaseScreen<TData> : BaseScreen where TData : struct
	{
		protected TData Data { get; private set; }

		internal void SetData(TData data)
		{
			Data = data;
			OnSetData();
		}

		protected virtual void OnSetData()
		{
		}
	}
}