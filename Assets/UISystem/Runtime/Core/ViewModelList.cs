using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UISystem.Runtime.Core
{
    public class ViewModelList<T> : IList<T>
    {
        private Action<(T item, int index)> _onAdd;
        private Action<(T item, int index)> _onRemove;
        private Action _onClear;

        private readonly List<T> _list = new();
        private readonly Dictionary<T, List<int>> _dictionary = new();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void Add(T item)
        {
            Debug.Assert(item != null, nameof(item) + " != null");

            _list.Add(item);

            if (_dictionary.ContainsKey(item))
            {
                _dictionary[item].Add(_list.Count - 1);
            }
            else
            {
                _dictionary.Add(item, new List<int> {_list.Count - 1});
            }

            _onAdd?.Invoke((item, Count - 1));
        }

        public void Clear()
        {
            _list.Clear();
            _dictionary.Clear();
            _onClear?.Invoke();
        }

        public bool Contains(T item)
        {
            Debug.Assert(item != null, nameof(item) + " != null");
            return _dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            Debug.Assert(item != null, nameof(item) + " != null");
            if (_dictionary.TryGetValue(item, out var indexes))
            {
                Remove(item, indexes.First());
                return true;
            }

            return false;
        }

        public int Count => _list.Count;
        public bool IsReadOnly => false;

        public int IndexOf(T item)
        {
            Debug.Assert(item != null, nameof(item) + " != null");
            if (_dictionary.TryGetValue(item, out var indexes))
            {
                return indexes.First();
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            var item = _list[index];
            Remove(item, index);
        }

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public void Connect(Action<(T item, int index)> onAdd, Action<(T item, int index)> onRemove, Action onClear)
        {
            var connected = _onAdd != null || _onRemove != null || _onClear != null;
            if (connected)
            {
                throw new Exception("Already connected");
            }

            _onAdd = onAdd;
            _onRemove = onRemove;
            _onClear = onClear;
        }

        public void Reset()
        {
            _onAdd = default;
            _onRemove = default;
            _onClear = default;
            _list.Clear();
            _dictionary.Clear();
        }

        private void Remove(T item, int index)
        {
            var indexes = _dictionary[item];
            indexes.Remove(index);
            if (indexes.Count == 0)
            {
                _dictionary.Remove(item);
            }

            _list.RemoveAt(index);
            _onRemove?.Invoke((item, index));
        }
    }
}