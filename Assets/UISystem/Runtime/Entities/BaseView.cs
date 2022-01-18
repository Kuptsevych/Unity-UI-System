using System.Collections.Generic;
using System.Linq;
using UISystem.Runtime.Assets;
using UISystem.Runtime.Components;
using UISystem.Runtime.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem.Runtime.Entities
{
    public abstract class BaseView : MonoBehaviour
    {
        [SerializeField] private bool _transparent;
        [SerializeField] private int _defaultLayer;
        [SerializeField] private TransitionType _preferOpenTransition;
        [SerializeField] private TransitionType _preferShowTransition;
        [SerializeField] private TransitionType _preferHideTransition;
        [SerializeField] private TransitionType _preferCloseTransition;
        [SerializeField] private int _spareUpWidth;
        [SerializeField] private int _spareDownWidth;
        [SerializeField, HideInInspector] private List<GameObject> _orderShiftObjects;

        private readonly HashSet<IOrderShift> _orderShifts = new();

        public bool Transparent => _transparent;

        public int DefaultLayer => _defaultLayer;

        public TransitionType PreferOpenTransition => _preferOpenTransition;

        public TransitionType PreferShowTransition => _preferShowTransition;

        public TransitionType PreferHideTransition => _preferHideTransition;

        public TransitionType PreferCloseTransition => _preferCloseTransition;

        public virtual float OpenDuration { get; }
        public virtual float ShowDuration { get; }
        public virtual float HideDuration { get; }
        public virtual float CloseDuration { get; }
        public (int maxUpShift, int maxDownShift) OrderWidth { get; private set; }
        internal Canvas Canvas { get; private set; }
        internal GraphicRaycaster Raycaster { get; private set; }

        public void SetSortingOrder(int sortingOrder)
        {
            Canvas.sortingOrder = sortingOrder;
            UpdateShiftObjects(_orderShifts, sortingOrder);
        }

        public void Init()
        {
            Canvas = GetComponent<Canvas>();
            Raycaster = GetComponent<GraphicRaycaster>();
            CreateViewModel();
            FillOrderShifts(_orderShiftObjects, _orderShifts);
            OrderWidth = CalculateOrderWidth(_orderShifts, _spareUpWidth, _spareDownWidth);
            OnInit();
        }

        protected abstract void CreateViewModel();

        internal void SetActive(bool isActive)
        {
            Canvas.enabled = isActive;
            Raycaster.enabled = isActive;
        }

        protected abstract void OnInit();

        public virtual void SetOpeningProgress(float normalizedTime)
        {
        }

        public virtual void SetShowingProgress(float normalizedTime)
        {
        }

        public virtual void SetHidingProgress(float normalizedTime)
        {
        }

        public virtual void SetClosingProgress(float normalizedTime)
        {
        }

        private static (int maxUpShift, int maxDownShift) CalculateOrderWidth(HashSet<IOrderShift> orderShifts, int spareUpWidth, int spareDownWidth)
        {
            var upShift = 0;
            var downShift = 0;
            foreach (var orderShift in orderShifts)
            {
                if (orderShift.Shift > 0)
                {
                    if (orderShift.Shift > upShift)
                    {
                        upShift = orderShift.Shift;
                    }
                }
                else
                {
                    if (orderShift.Shift < downShift)
                    {
                        downShift = orderShift.Shift;
                    }
                }
            }

            return (maxUpShift: upShift + spareUpWidth, maxDownShift: downShift + spareDownWidth);
        }

        private static void FillOrderShifts(List<GameObject> orderShiftObjects, HashSet<IOrderShift> orderShifts)
        {
            foreach (var orderShiftObject in orderShiftObjects)
            {
                orderShifts.Add(orderShiftObject.GetComponent<IOrderShift>());
            }
        }

        private static void UpdateShiftObjects(HashSet<IOrderShift> orderShifts, int canvasOrder)
        {
            foreach (var orderShift in orderShifts)
            {
                orderShift.SetOrder(orderShift.Shift + canvasOrder);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UIAssetsService.CheckAsset(this);
            CollectOrderShiftObjects();
        }

        private void CollectOrderShiftObjects()
        {
            var orderShiftComponents = GetComponentsInChildren<IOrderShift>(true);

            if (orderShiftComponents.Length > 0)
            {
                _orderShiftObjects.AddRange(orderShiftComponents.Select(x => x.GameObject));
            }
        }
#endif
    }
}