using System;
using System.Collections.Generic;

namespace UISystem.Runtime.Core
{
    public class ViewModelValue<T>
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

        public ViewModelValue()
        {
            _value = default;
        }

        public ViewModelValue(T init)
        {
            _value = init;
        }

        public void Set(T value)
        {
            Value = value;
        }

        public void Connect(Action<T> onChanged)
        {
            var connected = _onChanged != null;
            if (connected)
            {
                throw new Exception("Already connected");
            }

            _onChanged = onChanged;
            _onChanged.Invoke(Value);
        }

        public void Reset()
        {
            _onChanged = default;
            _value = default;
        }
    }
}