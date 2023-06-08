using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RJ.UISystem.Editor.Tools")]

namespace UISystem.Runtime.Entities
{
	public interface IViewModelItem
	{
		void Reset();
	}
}