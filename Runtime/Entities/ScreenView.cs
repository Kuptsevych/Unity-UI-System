namespace Entities
{
    public abstract class ScreenView<TViewModel> : BaseScreenView where TViewModel : BaseViewModel, new()
    {
        public TViewModel ViewModel { get; private set; }

        protected sealed override void CreateViewModel()
        {
            ViewModel = new TViewModel();
        }
    }
}