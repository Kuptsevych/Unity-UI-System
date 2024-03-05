using System;
using System.Collections.Generic;

namespace Entities
{
	public class ViewModelValue<T> : IViewModelItem
	{
		private T _value;

		private Action<T> _onChanged;

		public T Value
		{
			get => _value;
			set
			{
				if (!EqualityComparer<T>.Default.Equals(_value, value))
				{
					_value = value;
					_onChanged?.Invoke(value);
				}
			}
		}

		public ViewModelValue(T init = default)
		{
			_value = init;
		}

		public void Connect(Action<T> onChanged)
		{
			var connected = _onChanged != null;
			if (connected)
			{
				throw new Exception("Already connected");
			}

			_onChanged = onChanged;
			if (typeof(T).IsValueType || !EqualityComparer<T>.Default.Equals(_value, default))
			{
				_onChanged.Invoke(Value);
			}
		}

		public void Reset()
		{
			_onChanged = default;
			_value = default;
		}
		
		public void ForceUpdate()
		{
			_onChanged?.Invoke(Value);
		}
	}
}