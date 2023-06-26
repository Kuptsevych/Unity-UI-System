using System;
using UnityEngine;

namespace Entities
{
	public abstract class BaseView : MonoBehaviour
	{
		public bool Initialized { get; private set; }

		public void Init()
		{
			if (Initialized)
			{
				throw new Exception($"{GetType().Name} already initialized");
			}

			Initialized = true;

			CreateViewModel();
			OnInit();
		}

		protected abstract void CreateViewModel();

		protected abstract void OnInit();

#if UNITY_EDITOR
		private void OnValidate()
		{
			Validate();
		}

		protected virtual void Validate()
		{
		}
#endif
	}
}