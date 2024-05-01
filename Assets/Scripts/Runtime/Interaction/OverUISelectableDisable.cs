using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VENTUS.Interaction.Core.Interaction;

public class OverUISelectableDisable : MonoBehaviour, IInteractionActivatable
{
    [SerializeField] private List<InteractionTypes> _interactionTypes;
    [SerializeField] private XRRayInteractor _rayInteractor;
    [SerializeField] private LayerMask _overUILayerMask;
    [SerializeField] private LayerMask _overSelectableLayerMask;
    
    //needed: ray interactor as origin, selectable
    private bool _updateBehaviour;
    
    private void Awake()
    {
        foreach (var interactionTypes in _interactionTypes)
        {
            InteractionTracking.AddElement(interactionTypes, this);
        }
    }

    private void OnDestroy()
    {
        foreach (var interactionTypes in _interactionTypes)
        {
            InteractionTracking.RemoveElement(interactionTypes, this);
        }
    }

    public void OnEnableInteraction()
    {
        _updateBehaviour = true;
    }

    public void OnDisableInteraction()
    {
        _updateBehaviour = false;
    }
    
    private void Update()
    {
        if (!_updateBehaviour) return;

        _rayInteractor.raycastMask = _rayInteractor.IsOverUIGameObject() ? _overUILayerMask : _overSelectableLayerMask;
    }
}
