namespace Entities
{
	public abstract class View<TViewModel> : BaseView where TViewModel : BaseViewModel, new()
	{
		public TViewModel ViewModel { get; private set; }

		protected sealed override void CreateViewModel()
		{
			ViewModel = new TViewModel();
		}
	}
}