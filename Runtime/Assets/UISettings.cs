using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
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
		[SerializeField] private List<string> _layers;
		[SerializeField] private bool _createCamera = true;
		[SerializeField] private int _hintsLayer;
		[SerializeField] private CanvasScaler.ScaleMode _scaleMode;
		[SerializeField] private CanvasScaler.ScreenMatchMode _screenMatchMode;
		[SerializeField] private Vector2 _referenceResolution;

		[Tooltip("0 - width, 1 - height")] [Range(0, 1)] [SerializeField]
		private float _matchWidthOrHeight;

		//todo add drawer with layer names
		[SerializeField] private List<int> _nonBlockingLayers;

		public string RootName => _rootName;
		public int CameraDepth => _cameraDepth;
		public LayerMask CullingMask => _cullingMask;
		public int SortingOrderSpace => _sortingOrderSpace;

		public Dictionary<(Type, Type, bool), TransitionType> GetDefinedTransitions() =>
			_definedTransitionsAsset.GetDefinedTransitions();

		public List<TransitionType> TransitionTypesOrder => _transitionTypesOrder;
		public int LayersCount => _layers.Count;
		public bool CreateCamera => _createCamera;
		public int HintsLayer => _hintsLayer;
		public CanvasScaler.ScaleMode ScaleMode => _scaleMode;
		public CanvasScaler.ScreenMatchMode ScreenMatchMode => _screenMatchMode;
		public Vector2 ReferenceResolution => _referenceResolution;
		public IReadOnlyList<string> Layers => _layers;
		public float MatchWidthOrHeight => _matchWidthOrHeight;
		public IReadOnlyList<int> NonBlockingLayers => _nonBlockingLayers;
	}
}