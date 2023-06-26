using System;
using UnityEngine;

namespace Components
{
	public abstract class BaseComponent<T> : MonoBehaviour
	{
		private bool _initialized;
		protected T Owner { get; private set; }

		protected virtual void OnInitialized()
		{
		}

		protected abstract void OnReady();
		protected abstract void OnCleanUp();
		protected abstract void OnUpdate(float deltaTime);

		internal void Init(T owner)
		{
			if (!_initialized)
			{
				_initialized = true;
				Owner = owner;
				OnInitialized();
			}
			else
			{
				throw new Exception($"Extension of type {GetType()} already initialized");
			}
		}

		internal void UpdateComponent(float deltaTime)
		{
			OnUpdate(deltaTime);
		}

		internal void CleanUp()
		{
			OnCleanUp();
		}

		internal void Ready()
		{
			OnReady();
		}
	}
}