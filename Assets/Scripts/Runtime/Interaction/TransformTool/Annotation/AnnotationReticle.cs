using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using VENTUS.Interaction.Core.Controller;
using VENTUS.Interaction.TransformTool.UI.View.Annotation;

namespace VENTUS.Interaction.TransformTool.Annotation
{
    public class AnnotationReticle : MonoBehaviour
    {
        [SerializeField] private List<InteractFilter> _rayInteractor;
        [SerializeField] private List<MonoBehaviour> _disabledElements;
        [SerializeField] private float _lerpFactor = 0.2f;

        private AnnotationViewCreation _annotationViewCreation;
        private GameObject _instantiatedReticleVisual;
        private Image _reticleImage;
        private bool _isActive;
        private bool _isCurrentlyPlaceable;
        
        private void Awake()
        {
            AnnotationViewCreation.OnEnterAnnotationPlacement += OnEnterAnnotationPlacement;
            AnnotationViewCreation.OnExitAnnotationPlacement += OnExitAnnotationPlacement;
        }

        private void OnDestroy()
        {
            AnnotationViewCreation.OnEnterAnnotationPlacement -= OnEnterAnnotationPlacement;
            AnnotationViewCreation.OnExitAnnotationPlacement -= OnExitAnnotationPlacement;
        }

        private void OnEnterAnnotationPlacement(AnnotationViewCreation annotationViewCreation, GameObject reticleVisual)
        {
            _annotationViewCreation = annotationViewCreation;
            _instantiatedReticleVisual = Instantiate(reticleVisual);
            _reticleImage = _instantiatedReticleVisual.GetComponentInChildren<Image>();
            _isActive = true;
        }
        
        private void OnExitAnnotationPlacement()
        {
            Destroy(_instantiatedReticleVisual);
            _isActive = false;

            if (_disabledElements is { Count: > 0 })
                _disabledElements.ForEach(x => x.enabled = true);
        }

        private void Update()
        {
            if (!_isActive) return;

            bool hasRaycastHit = _annotationViewCreation.IsValidPlacementRaycast(transform, out RaycastHit raycastHit, out _);
            bool canInteract = _rayInteractor.All(x => x.CanInteract());
            
            if (_disabledElements is { Count: > 0 })
                _disabledElements.ForEach(x => x.enabled = canInteract);
            _instantiatedReticleVisual.SetActive(hasRaycastHit && canInteract);

            if (!hasRaycastHit || !canInteract) return;

            _instantiatedReticleVisual.transform.DORotateQuaternion(Quaternion.LookRotation(Camera.main.transform.position - raycastHit.point), _lerpFactor)
                .SetLink(_instantiatedReticleVisual, LinkBehaviour.KillOnDestroy);
            _instantiatedReticleVisual.transform.DOMove(raycastHit.point, _lerpFactor)
                .SetLink(_instantiatedReticleVisual, LinkBehaviour.KillOnDestroy);

            bool isValidPlacement = _annotationViewCreation.IsValidObject(raycastHit.transform, out _) && _annotationViewCreation.IsValidTextInput(out _);
            
            if (isValidPlacement != _isCurrentlyPlaceable)
            {
                _reticleImage.color = isValidPlacement ? Color.green : Color.red;
                _isCurrentlyPlaceable = isValidPlacement;
            }
        }
    }
}
