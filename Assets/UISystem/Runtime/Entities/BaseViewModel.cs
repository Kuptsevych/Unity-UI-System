using UISystem.Runtime.Core;

namespace UISystem.Runtime.Entities
{
    public abstract class BaseViewModel
    {
        public ViewModelValue<bool> Visible = new(true);
        public ViewModelValue<bool> Enabled = new(true);
    }
}