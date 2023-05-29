using System;
using System.Collections;
using System.Collections.Generic;

namespace UISystem.Runtime.Entities
{
	public class ViewModelList<T> : IList<T>, IViewModelItem
	{
		private Action<(T item, int index)> _onAdd;
		private Action<(T item, int index)> _onRemove;
		private Action<(T item, int index)> _onChange;
		private Action _onClear;

		private readonly List<T> _list = new();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public void AddRange(IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				Add(item);
			}
		}

		public void Add(T item)
		{
			_list.Add(item);
			_onAdd?.Invoke((item, Count - 1));
		}

		public void Clear()
		{
			_list.Clear();
			_onClear?.Invoke();
		}

		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			for (var i = _list.Count - 1; i >= 0; i--)
			{
				if (EqualityComparer<T>.Default.Equals(_list[i], item))
				{
					RemoveAt(i);
					_onRemove.Invoke((item, i));
					return true;
				}
			}

			return false;
		}

		public int Count => _list.Count;
		public bool IsReadOnly => false;

		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_list.Insert(index, item);
			_onAdd?.Invoke((item, index));
		}

		public void RemoveAt(int index)
		{
			var item = _list[index];
			_list.RemoveAt(index);
			_onRemove?.Invoke((item, index));
		}

		public T this[int index]
		{
			get => _list[index];
			set
			{
				if (!EqualityComparer<T>.Default.Equals(_list[index], value))
				{
					_list[index] = value;
					_onChange?.Invoke((value, index));
				}
			}
		}

		public void Connect(Action<(T item, int index)> onAdd, Action<(T item, int index)> onRemove, Action<(T item, int index)> onChange, Action onClear)
		{
			var connected = _onAdd != null || _onRemove != null || _onClear != null || _onChange != null;
			if (connected)
			{
				throw new Exception("Already connected");
			}

			_onAdd = onAdd;
			_onRemove = onRemove;
			_onClear = onClear;
			_onChange = onChange;
		}

		public void Reset()
		{
			_onAdd = default;
			_onRemove = default;
			_onClear = default;
			_onChange = default;
			_list.Clear();
		}
	}
}