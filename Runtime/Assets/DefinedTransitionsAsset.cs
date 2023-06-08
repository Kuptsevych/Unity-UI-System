using System;
using System.Collections.Generic;
using UISystem.Runtime.Core;
using UnityEngine;

namespace UISystem.Runtime.Assets
{
    [CreateAssetMenu(fileName = "defined_transitions", menuName = "UI/Defined transitions", order = 0)]
    public class DefinedTransitionsAsset : ScriptableObject
    {
        [Serializable]
        private struct DefinedTransition
        {
            public string ScreenType;
            public string NextScreenType;
            public TransitionType PreferredTransition;
            public bool IsOpen;
        }

        [SerializeField] private List<DefinedTransition> _definedTransitions = new();

        public Dictionary<(Type, Type, bool), TransitionType> GetDefinedTransitions()
        {
            return GetDefinedTransitions(_definedTransitions);
        }

        private static Dictionary<(Type, Type, bool), TransitionType> GetDefinedTransitions(List<DefinedTransition> definedTransitions)
        {
            var result = new Dictionary<(Type, Type, bool), TransitionType>();

            foreach (var definedTransition in definedTransitions)
            {
                result.Add(
                    (Type.GetType(definedTransition.ScreenType), Type.GetType(definedTransition.NextScreenType), definedTransition.IsOpen),
                    definedTransition.PreferredTransition);
            }

            return result;
        }
    }
}