using System.Threading.Tasks;
using Core;

namespace Entities
{
	public abstract class Screen<TScreen, TViewModel, TView, TData> : BaseScreen<TData>
		where TViewModel : BaseViewModel, new()
		where TView : ScreenView<TViewModel>
		where TScreen : BaseScreen<TData>, new()
		where TData : struct
	{
		protected TViewModel ViewModel { get; private set; }

		protected TView View => (TView) BaseScreenView;

		public static async Task<TScreen> Open(TData data, bool instant = false)
		{
			return await UI.Open<TScreen, TData>(data, instant);
		}

		public static void Close(TScreen screen, bool instant = false)
		{
			UI.Close(screen, instant);
		}

		internal sealed override void OnConnectView()
		{
			ViewModel = ((TView) BaseScreenView).ViewModel;
			OnConnect();
		}

		protected virtual void OnConnect()
		{
		}
	}
}