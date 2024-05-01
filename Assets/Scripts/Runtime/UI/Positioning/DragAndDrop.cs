using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using VENTUS.DataStructures.Variables;

namespace VENTUS.UI.Positioning
{
    public class DragAndDrop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        #region fields
        
        [SerializeField] private RectTransform _targetTransform;
        [SerializeField] private Image _image;
        [SerializeField] private float _smoothSpeed = 15f;
        [SerializeField] private float _changeAlphaSpeed = 1f;
        [SerializeField] private float _maximumAlpha = 0.3f;

        private bool _isDragging;
        private Vector3 _startOffset;
        private float _grabOriginDistance;
        private XRBaseControllerInteractor _draggingInteractor;
        
        private IEnumerator _showCoroutine;
        private IEnumerator _hideCoroutine;

        private HashSet<XRBaseControllerInteractor> _hoveringInteractors = new();

        #endregion

        #region lifecycle
        
        private void Awake()
        {
            _targetTransform ??= GetComponent<RectTransform>();
        }
        
        private void Update()
        {
            if (!_isDragging) return;

            var interactorForwardVector = _draggingInteractor.transform.forward;
            var directionVector = Quaternion.LookRotation(interactorForwardVector) * Vector3.forward * _grabOriginDistance;
            var targetPosition = _draggingInteractor.transform.position + directionVector + _startOffset;
            _targetTransform.position = Vector3.Lerp(_targetTransform.position, targetPosition, _smoothSpeed * Time.deltaTime);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isDragging
                || eventData is not TrackedDeviceEventData { interactor: XRBaseControllerInteractor xrInteractor }) 
                return;
            
            _isDragging = true;
            _draggingInteractor = xrInteractor;
            _grabOriginDistance = eventData.pointerCurrentRaycast.distance;
            _startOffset = _targetTransform.position - eventData.pointerCurrentRaycast.worldPosition;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isDragging
                || eventData is not TrackedDeviceEventData { interactor: XRBaseControllerInteractor xrInteractor }
                || xrInteractor != _draggingInteractor)
                return;

            _isDragging = false;
            _draggingInteractor = null;
            _grabOriginDistance = 0;
            
            if (_hoveringInteractors.Count > 0) return;
            _hideCoroutine = FadeImageAlpha(0);
            StartCoroutine(_hideCoroutine);
            
            if (_showCoroutine == null) return;
            StopCoroutine(_showCoroutine);
            _showCoroutine = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData is not TrackedDeviceEventData { interactor: XRBaseControllerInteractor xrInteractor }
                || !_hoveringInteractors.Add(xrInteractor))
                return;

            if (_hoveringInteractors.Count > 1) return;
            _showCoroutine = FadeImageAlpha(_maximumAlpha);
            StartCoroutine(_showCoroutine);

            if (_hideCoroutine == null) return;
            StopCoroutine(_hideCoroutine);
            _hideCoroutine = null;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData is not TrackedDeviceEventData { interactor: XRBaseControllerInteractor xrInteractor }
                || !_hoveringInteractors.Remove(xrInteractor)
                || _isDragging)
                return;
            
            if (_hoveringInteractors.Count != 0) return;
            _hideCoroutine = FadeImageAlpha(0);
            StartCoroutine(_hideCoroutine);

            if (_showCoroutine == null) return;
            StopCoroutine(_showCoroutine);
            _showCoroutine = null;
        }

        private IEnumerator FadeImageAlpha(float targetAlpha)
        {
            float time = 0;
            var startAlpha = _image.color.a;
            while (time <= _changeAlphaSpeed)
            {
                var t = math.remap(0, _changeAlphaSpeed, startAlpha, targetAlpha, time);
                _image.color = new(_image.color.r, _image.color.g, _image.color.b, t);
                time += Time.deltaTime;
                yield return null;
            }
        }

        #endregion
    }
}
