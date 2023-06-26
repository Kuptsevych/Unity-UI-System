using UnityEngine;

namespace Core
{
	internal class Root : MonoBehaviour
	{
		private void Update()
		{
			UI.Update(Time.deltaTime);
		}
	}
}