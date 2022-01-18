using System;
using System.Collections.Generic;

namespace UISystem.Runtime.Core
{
    public readonly struct ScreensTransitionData
    {
        public readonly Dictionary<(Type, Type, bool), TransitionType> DefinedTransitions;
        public readonly Dictionary<TransitionType, int> TransitionsOrder;

        public ScreensTransitionData(Dictionary<(Type, Type, bool), TransitionType> definedTransitions, List<TransitionType> transitionTypesOrder)
        {
            DefinedTransitions = definedTransitions;
            TransitionsOrder = new();

            for (var i = 0; i < transitionTypesOrder.Count; i++)
            {
                TransitionsOrder.Add(transitionTypesOrder[i], i);
            }
        }
    }
}