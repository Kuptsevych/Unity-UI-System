using System;
using UISystem.Runtime.Core;

namespace UISystem.Runtime.Entities
{
    public abstract class BaseScreen
    {
        public event Action<(BaseScreen screen, ScreenState prevState, ScreenState newState)> OnStateChanged;

        private ScreenState _state;

        internal BaseView BaseView { get; private set; }

        public ScreenState State
        {
            get => _state;
            private set => SwitchState(this, value);
        }

        internal void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        internal void ConnectView(BaseView baseView)
        {
            BaseView = baseView;
            OnConnectView();
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

        protected abstract void OnUpdate(float deltaTime);

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

        private static ScreenSwitchStateHandler Open(BaseScreen screen, bool instant)
        {
            if (instant || screen.BaseView.CloseDuration <= 0)
            {
                screen.State = ScreenState.Opened;
                screen.OnOpen();
                screen.BaseView.SetActive(true);
                return default;
            }

            screen.State = ScreenState.Opening;
            screen.BaseView.Raycaster.enabled = false;
            screen.OnOpening();
            return new ScreenSwitchStateHandler(screen, screen.BaseView.OpenDuration, screen.State);
        }

        private static ScreenSwitchStateHandler Show(BaseScreen screen, bool instant)
        {
            if (instant || screen.BaseView.CloseDuration <= 0)
            {
                screen.State = ScreenState.Opened;
                screen.OnShow();
                screen.BaseView.Raycaster.enabled = true;
                return default;
            }

            screen.State = ScreenState.Showing;
            screen.BaseView.Raycaster.enabled = false;
            screen.OnShowing();
            return new ScreenSwitchStateHandler(screen, screen.BaseView.ShowDuration, screen.State);
        }

        private static ScreenSwitchStateHandler Hide(BaseScreen screen, bool instant)
        {
            if (instant || screen.BaseView.CloseDuration <= 0)
            {
                screen.State = ScreenState.Hided;
                screen.OnHide();
                screen.BaseView.Raycaster.enabled = false;
                return default;
            }

            screen.State = ScreenState.Hiding;
            screen.BaseView.Raycaster.enabled = false;
            screen.OnHiding();
            return new ScreenSwitchStateHandler(screen, screen.BaseView.HideDuration, screen.State);
        }

        private static ScreenSwitchStateHandler Close(BaseScreen screen, bool instant)
        {
            if (instant || screen.BaseView.CloseDuration <= 0)
            {
                screen.State = ScreenState.Closed;
                screen.BaseView.SetActive(false);
                screen.OnClose();
                return default;
            }

            screen.State = ScreenState.Closing;
            screen.BaseView.Raycaster.enabled = false;
            screen.OnClosing();
            return new ScreenSwitchStateHandler(screen, screen.BaseView.CloseDuration, screen.State);
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
                    screen.BaseView.SetOpeningProgress(normalizedTime);
                    break;
                case ScreenState.Showing:
                    screen.BaseView.SetShowingProgress(normalizedTime);
                    break;
                case ScreenState.Hiding:
                    screen.BaseView.SetHidingProgress(normalizedTime);
                    break;
                case ScreenState.Closing:
                    screen.BaseView.SetClosingProgress(normalizedTime);
                    break;
            }
        }

        private static void CompleteSwitchState(BaseScreen screen)
        {
            switch (screen.State)
            {
                case ScreenState.Opening:
                    screen.State = ScreenState.Opened;
                    screen.BaseView.SetActive(true);
                    screen.OnOpen();
                    break;
                case ScreenState.Showing:
                    screen.State = ScreenState.Opened;
                    screen.BaseView.Raycaster.enabled = true;
                    screen.OnShow();
                    break;
                case ScreenState.Hiding:
                    screen.State = ScreenState.Hided;
                    screen.BaseView.Raycaster.enabled = false;
                    screen.OnHide();
                    break;
                case ScreenState.Closing:
                    screen.State = ScreenState.Closed;
                    screen.BaseView.SetActive(false);
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