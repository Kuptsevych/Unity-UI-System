using System.Collections.Generic;

namespace Entities
{
	public abstract class BaseViewModel : IViewModelItem
	{
		public ViewModelValue<bool> Visible = new(true);
		public ViewModelValue<bool> Enabled = new(true);

		private readonly List<IViewModelItem> _fields = new();

		protected BaseViewModel()
		{
			foreach (var fieldInfo in GetType().GetFields())
			{
				var field = (IViewModelItem) fieldInfo.GetValue(this);
				_fields.Add(field);
			}
		}

		public void Reset()
		{
			foreach (var viewModelField in _fields)
			{
				viewModelField.Reset();
			}
		}
	}
}