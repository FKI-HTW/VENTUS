using System;
using UnityEngine;
using UnityEngine.UI;

namespace VENTUS.Interaction.Core.Interaction
{
    [RequireComponent(typeof(Toggle))]
    public class InteractionToggle : MonoBehaviour
    {
        [SerializeField] private InteractionTypes _interactionType;
    
        private Toggle _toggle;
        private bool _isToggleEnabled;
    
        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.isOn = InteractionTracking.CurrentInteractionType == _interactionType;
            _toggle.onValueChanged.AddListener(UpdateInteraction);
        }

        private void OnEnable()
        {
            if (!_isToggleEnabled) return;
            
            SetInteraction(true);
        }

        private void OnDisable()
        {
            if (!_isToggleEnabled) return;
            
            SetInteraction(false);
        }

        private void OnDestroy()
        {
            _toggle.onValueChanged.RemoveListener(UpdateInteraction);
        }

        private void UpdateInteraction(bool isToggleEnabled)
        {
            _isToggleEnabled = isToggleEnabled;

            SetInteraction(isToggleEnabled);
        }

        private void SetInteraction(bool isToggleEnabled)
        {
            if (!isToggleEnabled && InteractionTracking.CurrentInteractionType == _interactionType)
            {
                InteractionTracking.ChangeCurrentInteraction(InteractionTypes.None);
                return;
            }

            InteractionTracking.ChangeCurrentInteraction(_interactionType);
        }
    }
}
