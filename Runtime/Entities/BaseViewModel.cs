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
				if (fieldInfo.GetValue(this) is IViewModelItem viewModelItem)
				{
					_fields.Add(viewModelItem);
				}
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