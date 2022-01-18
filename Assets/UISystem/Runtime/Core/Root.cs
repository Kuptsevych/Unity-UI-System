using UnityEngine;

namespace UISystem.Runtime.Core
{
    internal class Root : MonoBehaviour
    {
        private ScreenManager _screenManager;

        public void Connect(ScreenManager screenManager)
        {
            _screenManager = screenManager;
        }

        private void Update()
        {
            _screenManager.Update(Time.deltaTime);
        }
    }
}