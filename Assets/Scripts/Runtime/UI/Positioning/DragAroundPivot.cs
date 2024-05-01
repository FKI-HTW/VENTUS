using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit;
using VENTUS.DataStructures.Variables;

namespace VENTUS.UI.Positioning
{
    public class DragAroundPivot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
        #region fields

        [SerializeField] private Transform _dragObject;
        [SerializeField] private CameraVariable _pivot;
        [SerializeField] private float _distance;
        [SerializeField] private float _heightDifference;
        [SerializeField] private float _startAngle;
        
        [Header("Drag Panel Visuals")]
        [SerializeField] private Image _image;
        [SerializeField] private float _changeAlphaSpeed = 1f;
        [SerializeField] private float _maximumAlpha = 0.3f;

        private Transform _transformDriver;
        private bool _isDragging;
		private float _startRotation;
        private float _angle;
        private float _dragAngle;
        private Vector2 _dragStartPos;
        
        private IEnumerator _showCoroutine;
        private IEnumerator _hideCoroutine;

        private HashSet<XRBaseControllerInteractor> _hoveringInteractors = new();
        
		#endregion

		#region lifecycle

		private void OnEnable()
		{
			if (_dragObject == null)
				_dragObject = transform;

			_isDragging = false;
			_angle = _dragAngle = 0;
			_startRotation = _pivot.Get().transform.position.y + _startAngle;
		}

		private void OnDisable()
		{
			_isDragging = false;
			_angle = _dragAngle = _startRotation = 0;
		}

		private void Update()
        {
            Vector3 pivotPos = _pivot.Get().transform.position;

            if (_isDragging && _transformDriver != null)
			{
                Vector2 cameraPos2D = new(pivotPos.x, pivotPos.z);
                Vector2 handPos2D = new(_transformDriver.position.x, _transformDriver.position.z);

                Vector2 initialPos = _dragStartPos - cameraPos2D;
                Vector2 targetPos = handPos2D - cameraPos2D;

                _dragAngle = -Vector2.SignedAngle(initialPos, targetPos);
            }

            Vector3 newPosition = pivotPos + Vector3.forward * _distance + Vector3.down * _heightDifference;
            newPosition = Quaternion.Euler(0, _startRotation + _angle + _dragAngle, 0) * (newPosition - pivotPos) + pivotPos;
            newPosition.y = Mathf.Max(0.2f, newPosition.y);
            _dragObject.position = newPosition;
        }

		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData is TrackedDeviceEventData { interactor: XRBaseControllerInteractor xrInteractor })
			{
				_transformDriver = xrInteractor.transform;
				_dragStartPos = new(xrInteractor.transform.position.x, xrInteractor.transform.position.z);
				_isDragging = true;
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_transformDriver = null;
            _isDragging = false;
            _dragStartPos = Vector2.zero;
            _angle += _dragAngle;
            _dragAngle = 0;
            
            if (_hoveringInteractors.Count > 0) return;
            _hideCoroutine = FadeImageAlpha(0);
            StartCoroutine(_hideCoroutine);
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
