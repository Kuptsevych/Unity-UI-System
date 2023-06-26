using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RJ.UISystem.Editor.Tools")]

namespace Entities
{
	public interface IViewModelItem
	{
		void Reset();
	}
}