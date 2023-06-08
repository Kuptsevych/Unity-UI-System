using System;
using UISystem.Runtime.Entities;

namespace UISystem.Runtime.Core
{
    public class ScreensTransition
    {
        private struct Transition
        {
            private readonly TransitionType _type;
            private ScreenSwitchStateHandler _currentScreenSwitchHandler;
            private ScreenSwitchStateHandler _nextScreenSwitchHandler;
            private Action _callback;

            public bool InProgress => _currentScreenSwitchHandler.InProgress || _nextScreenSwitchHandler.InProgress;

            public Transition(TransitionType type, ScreenSwitchStateHandler currentScreenSwitchHandler,
                ScreenSwitchStateHandler nextScreenSwitchHandler, Action callback)
            {
                _type = type;
                _currentScreenSwitchHandler = currentScreenSwitchHandler;
                _nextScreenSwitchHandler = nextScreenSwitchHandler;
                _callback = callback;
            }

            public void Update(float deltaTime)
            {
                if (InProgress)
                {
                    if (_type == TransitionType.Parallel)
                    {
                        _currentScreenSwitchHandler.Update(deltaTime);
                        _nextScreenSwitchHandler.Update(deltaTime);
                    }
                    else if (_type == TransitionType.Sequential)
                    {
                        if (_nextScreenSwitchHandler.InProgress)
                        {
                            _nextScreenSwitchHandler.Update(deltaTime);
                        }
                        else if (_currentScreenSwitchHandler.InProgress)
                        {
                            _currentScreenSwitchHandler.Update(deltaTime);
                        }
                    }
                }
                else
                {
                    _callback?.Invoke();
                    _callback = null;
                }
            }

            public void ForceComplete()
            {
                _currentScreenSwitchHandler.ForceComplete();
                _nextScreenSwitchHandler.ForceComplete();
            }
        }

        private Transition _currentTransition;

        private readonly ScreensTransitionData _data;

        public ScreensTransition(ScreensTransitionData data)
        {
            _data = data;
        }

        public void Update(float deltaTime)
        {
            if (_currentTransition.InProgress)
            {
                _currentTransition.Update(deltaTime);
            }
        }

        public void StartOpenTransition(BaseScreen screen, BaseScreen nextScreen, bool instant, Action onComplete = null)
        {
            _currentTransition = StartOpenTransition(_currentTransition, _data, screen, nextScreen, instant, onComplete);
        }

        public void StartCloseTransition(BaseScreen screen, BaseScreen nextScreen, bool instant, Action onComplete = null)
        {
            _currentTransition = StartCloseTransition(_currentTransition, _data, screen, nextScreen, instant, onComplete);
        }

        private static TransitionType GetTransitionType(ScreensTransitionData data, BaseScreen screen, BaseScreen nextScreen, bool isOpen)
        {
            if (screen == null)
            {
                return isOpen ? nextScreen.BaseView.PreferOpenTransition : nextScreen.BaseView.PreferShowTransition;
            }

            if (data.DefinedTransitions.TryGetValue((screen.GetType(), nextScreen.GetType(), isOpen),
                    out var transitionType))
            {
                return transitionType;
            }

            if (isOpen)
            {
                var hidePreferTransition = screen.BaseView.PreferHideTransition;
                var openPreferTransition = nextScreen.BaseView.PreferOpenTransition;
                return data.TransitionsOrder[openPreferTransition] < data.TransitionsOrder[hidePreferTransition]
                    ? openPreferTransition
                    : hidePreferTransition;
            }

            var closePreferTransition = screen.BaseView.PreferCloseTransition;
            var showPreferTransition = nextScreen.BaseView.PreferShowTransition;
            return data.TransitionsOrder[closePreferTransition] < data.TransitionsOrder[showPreferTransition] ? closePreferTransition : showPreferTransition;
        }

        private static Transition StartOpenTransition(Transition currentTransition, ScreensTransitionData data, BaseScreen screen, BaseScreen nextScreen, bool instant, Action onComplete)
        {
            if (currentTransition.InProgress)
            {
                currentTransition.ForceComplete();
            }

            var transitionType = instant ? TransitionType.Instant : GetTransitionType(data, screen, nextScreen, true);

            var instantTransition = transitionType == TransitionType.Instant;

            var nextScreenSwitchHandler = nextScreen.Open(instantTransition);

            ScreenSwitchStateHandler currentScreenSwitchHandler = default;
            if (screen is {State: ScreenState.Opened})
            {
                currentScreenSwitchHandler = screen.Hide(instantTransition);
            }

            return new Transition(transitionType, currentScreenSwitchHandler, nextScreenSwitchHandler, onComplete);
        }
        
        private static Transition StartCloseTransition(Transition currentTransition, ScreensTransitionData data, BaseScreen screen, BaseScreen nextScreen, bool instant, Action onComplete)
        {
            if (currentTransition.InProgress)
            {
                currentTransition.ForceComplete();
            }

            var transitionType = instant ? TransitionType.Instant : GetTransitionType(data, screen, nextScreen, false);

            var instantTransition = transitionType == TransitionType.Instant;

            var currentScreenSwitchHandler = screen.Close(instantTransition);

            ScreenSwitchStateHandler nextScreenSwitchHandler = default;
            if (nextScreen is {State: ScreenState.Hided})
            {
                nextScreenSwitchHandler = nextScreen.Show(instantTransition);
            }

            return new Transition(transitionType, currentScreenSwitchHandler, nextScreenSwitchHandler, onComplete);
        }
    }
}