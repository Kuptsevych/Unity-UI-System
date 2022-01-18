using System;
using System.Collections.Generic;
using UISystem.Runtime.Core;
using UnityEngine;

namespace UISystem.Runtime.Assets
{
    [CreateAssetMenu(fileName = "ui_settings", menuName = "UI/Settings", order = 0)]
    public class UISettings : ScriptableObject
    {
        [SerializeField] private string _rootName = "UIRoot";
        [SerializeField] private int _cameraDepth = 1;
        [SerializeField] private LayerMask _cullingMask;
        [SerializeField] private int _sortingOrderSpace = 10;
        [SerializeField] private DefinedTransitionsAsset _definedTransitionsAsset;
        [SerializeField] private List<TransitionType> _transitionTypesOrder;
        [SerializeField] private int _layersCount = 10;

        public string RootName => _rootName;

        public int CameraDepth => _cameraDepth;

        public LayerMask CullingMask => _cullingMask;

        public int SortingOrderSpace => _sortingOrderSpace;

        public Dictionary<(Type, Type, bool), TransitionType> GetDefinedTransitions() =>
            _definedTransitionsAsset.GetDefinedTransitions();

        public List<TransitionType> TransitionTypesOrder => _transitionTypesOrder;
        public int LayersCount => _layersCount;
    }
}