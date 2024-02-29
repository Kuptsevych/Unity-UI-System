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
		
		public void Init<T>(T viewModel) where T : BaseViewModel
		{
			if (Initialized)
			{
				throw new Exception($"{GetType().Name} already initialized");
			}

			Initialized = true;

			SetViewModel(viewModel);
			
			OnInit();
		}

		protected abstract void CreateViewModel();
		protected abstract void SetViewModel<T>(T viewModel) where T : BaseViewModel;

		protected abstract void OnInit();
		protected abstract void OnSetViewModel();

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