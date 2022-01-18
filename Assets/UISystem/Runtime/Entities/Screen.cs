using UISystem.Runtime.Core;

namespace UISystem.Runtime.Entities
{
    public abstract class Screen<TScreen, TViewModel, TView, TData> : BaseScreen<TData>
        where TViewModel : BaseViewModel, new()
        where TView : View<TViewModel>
        where TScreen : BaseScreen<TData>, new()
        where TData : struct
    {
        protected TViewModel ViewModel { get; private set; }

        public static TScreen Open(TData data, bool instant = false)
        {
            return UI.Open<TScreen, TData>(data, instant);
        }

        public static void Close(TScreen screen, bool instant = false)
        {
            UI.Close(screen, instant);
        }

        internal override void OnConnectView()
        {
            ViewModel = ((TView) BaseView).ViewModel;
            OnConnect();
        }

        protected virtual void OnConnect()
        {
        }
    }
}