using UnityEngine;

namespace Components
{
	public interface IOrderShift
	{
		int Shift { get; }
		GameObject GameObject { get; }
		void SetOrder(int order);
	}
}