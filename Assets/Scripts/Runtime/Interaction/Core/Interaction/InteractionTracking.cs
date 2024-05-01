using System.Collections.Generic;
using UnityEngine;

namespace VENTUS.Interaction.Core.Interaction
{
    public static class InteractionTracking
    {
        public static InteractionTypes CurrentInteractionType = InteractionTypes.None;

        private static readonly Dictionary<InteractionTypes, List<IInteractionActivatable>> _lookup = new Dictionary<InteractionTypes, List<IInteractionActivatable>>();

        public static void AddElement(InteractionTypes interactionTypes, IInteractionActivatable interactionActivatable)
        {
            if (_lookup.TryGetValue(interactionTypes, out List<IInteractionActivatable> interactions))
            {
                interactions.Add(interactionActivatable);
            }
            else
            {
                _lookup.Add(interactionTypes, new List<IInteractionActivatable>{interactionActivatable});
            }
            
            if (CurrentInteractionType == interactionTypes)
            {
                interactionActivatable.OnEnableInteraction();
            }
        }
        
        public static void RemoveElement(InteractionTypes interactionTypes, IInteractionActivatable interactionActivatable)
        {
            if (!_lookup.TryGetValue(interactionTypes, out List<IInteractionActivatable> interactions))
            {
                Debug.LogWarning($"The requested {typeof(InteractionTypes)}.{interactionTypes} doesn't exist inside the {typeof(InteractionTracking)}");
                return;
            }

            if (!interactions.Remove(interactionActivatable))
            {
                Debug.LogWarning($"The requested {interactionActivatable} doesn't exist inside the {typeof(InteractionTracking)}");
            }

            if (interactions.Count == 0)
            {
                _lookup.Remove(interactionTypes);
                
                if (CurrentInteractionType == interactionTypes)
                {
                    interactionActivatable.OnDisableInteraction();
                }
            }
        }
        
        public static void ChangeCurrentInteraction(InteractionTypes interactionTypes)
		{
            if (interactionTypes == CurrentInteractionType)
                return;
            
            if (CurrentInteractionType != InteractionTypes.None && _lookup.ContainsKey(CurrentInteractionType))
            {
                _lookup[CurrentInteractionType].ForEach(x => x.OnDisableInteraction());
            }
            
            if (interactionTypes != InteractionTypes.None)
            {
                if (!_lookup.TryGetValue(interactionTypes, out List<IInteractionActivatable> newInteractions))
                {
                    Debug.LogWarning($"The requested {typeof(InteractionTypes)}.{interactionTypes} doesn't exist inside the {typeof(InteractionTracking)}");
                    return;
                }
                
                CurrentInteractionType = interactionTypes;
                newInteractions.ForEach(x => x.OnEnableInteraction());
            }
            else
            {
                CurrentInteractionType = interactionTypes;
            }
        }
    }
}
