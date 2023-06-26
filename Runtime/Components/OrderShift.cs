using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(Canvas))]
    public class OrderShift : MonoBehaviour, IOrderShift
    {
        [SerializeField] private int _shift;
        [SerializeField, HideInInspector] private Canvas _canvas;

        int IOrderShift.Shift => _shift;

        GameObject IOrderShift.GameObject => gameObject;

        void IOrderShift.SetOrder(int order)
        {
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = order;
        }
    }
}