using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Entities
{
    [Serializable]
    public abstract class BaseScreenView : BaseView
    {
        [Serializable]
        public struct BaseScreenViewSettings
        {
            [SerializeField] private bool _transparent;
            [SerializeField] private int _defaultLayer;
            [SerializeField] private TransitionType _preferOpenTransition;
            [SerializeField] private TransitionType _preferShowTransition;
            [SerializeField] private TransitionType _preferHideTransition;
            [SerializeField] private TransitionType _preferCloseTransition;
            [SerializeField] private int _spareUpWidth;
            [SerializeField] private int _spareDownWidth;

            public bool Transparent
            {
                get => _transparent;
                set => _transparent = value;
            }

            public int DefaultLayer => _defaultLayer;
            public TransitionType PreferOpenTransition => _preferOpenTransition;
            public TransitionType PreferShowTransition => _preferShowTransition;
            public TransitionType PreferHideTransition => _preferHideTransition;
            public TransitionType PreferCloseTransition => _preferCloseTransition;
            public int SpareUpWidth => _spareUpWidth;
            public int SpareDownWidth => _spareDownWidth;
        }

        [SerializeField] private BaseScreenViewSettings _settings;

        [SerializeField, HideInInspector, Space]
        private List<GameObject> _orderShiftObjects = new();

        private readonly HashSet<IOrderShift> _orderShifts = new();

        public bool Transparent
        {
            get => _settings.Transparent;
            set => _settings.Transparent = value;
        }

        public int DefaultLayer => _settings.DefaultLayer;

        public TransitionType PreferOpenTransition => _settings.PreferOpenTransition;

        public TransitionType PreferShowTransition => _settings.PreferShowTransition;

        public TransitionType PreferHideTransition => _settings.PreferHideTransition;

        public TransitionType PreferCloseTransition => _settings.PreferCloseTransition;

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

        protected sealed override void OnInit()
        {
            Canvas = GetComponent<Canvas>();
            Raycaster = GetComponent<GraphicRaycaster>();
            CreateViewModel();
            FillOrderShifts(_orderShiftObjects, _orderShifts);
            OrderWidth = CalculateOrderWidth(_orderShifts, _settings.SpareUpWidth, _settings.SpareDownWidth);
            OnScreenInit();
        }

        internal void UpdateView(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        protected abstract void OnScreenInit();

        protected virtual void OnUpdate(float deltaTime)
        {
        }

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
        protected sealed override void Validate()
        {
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