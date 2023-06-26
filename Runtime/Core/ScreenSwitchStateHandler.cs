using Entities;
using UnityEngine;

namespace Core
{
	public struct ScreenSwitchStateHandler
	{
		public readonly float Duration;
		public readonly ScreenState State;

		private readonly BaseScreen _screen;
		private float _timer;
		private bool _completed;

		public ScreenSwitchStateHandler(BaseScreen screen, float duration, ScreenState state)
		{
			Duration = duration;
			State = state;
			_screen = screen;
			_timer = 0;
			_completed = false;
		}

		public bool InProgress => _timer < Duration;

		public void Update(float deltaTime)
		{
			if (!InProgress)
			{
				if (!_completed)
				{
					ForceComplete();
				}

				return;
			}

			if (_screen != null && Duration > 0)
			{
				_timer += deltaTime;
				var progress = Mathf.Clamp01(_timer / Duration);
				_screen.SetSwitchStateProgress(progress);

				if (_timer >= Duration)
				{
					_completed = true;
					_screen.CompleteSwitchState();
				}
			}
		}

		public void ForceComplete()
		{
			_completed = true;
			if (_screen != null && !_completed)
			{
				_screen.SetSwitchStateProgress(1f);
				_screen.CompleteSwitchState();
			}
		}
	}
}