using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VENTUS.Interaction.TransformTool.Core.Selection;
using VENTUS.Interaction.TransformTool.Core.View;
using VENTUS.Interaction.TransformTool.Interaction;

namespace VENTUS.Interaction.TransformTool.UI.View
{
    public enum SelectionType { Move, Rotate, Scale, Grab }
    
    public class TransformView : MonoBehaviour
    {
        #region Fields

        [SerializeField] private SidebarSelectionView _sidebarSelectionView;
        
        [Header("Transform View")]
        [SerializeField] private SelectionType _entrySelectionType;
        [SerializeField] private int _numberDigitAmount;
        [SerializeField] private GameObject _gizmo;

        [Header("Position")] 
        [SerializeField] private Toggle _positionSwapInteractionToggle;
        [SerializeField] private TMP_Text _positionX;
        [SerializeField] private TMP_Text _positionY;
        [SerializeField] private TMP_Text _positionZ;
    
        [Header("Rotation")]
        [SerializeField] private Toggle _rotationSwapInteractionToggle;
        [SerializeField] private TMP_Text _rotationX;
        [SerializeField] private TMP_Text _rotationY;
        [SerializeField] private TMP_Text _rotationZ;
    
        [Header("Scale")]
        [SerializeField] private Toggle _scaleSwapInteractionToggle;
        [SerializeField] private TMP_Text _scaleX;
        [SerializeField] private TMP_Text _scaleY;
        [SerializeField] private TMP_Text _scaleZ;
        
        private XRTransformerInteractable _interactable;
        private GameObject _instantiatedGizmo;
        
        #endregion
    
        #region Unity Lifecycle

        private void Awake()
        {
            _sidebarSelectionView.RegisterAction(Initialize, SidebarSelectionViewEventType.Initialize);
            _sidebarSelectionView.RegisterAction(OnBeforeEnter, SidebarSelectionViewEventType.OnBeforeEnter);
            _sidebarSelectionView.RegisterAction(OnAfterRelease, SidebarSelectionViewEventType.OnAfterExit, 
                SidebarSelectionViewEventType.OnAfterExitImmediate);
        }

        private void OnDestroy()
        {
            _sidebarSelectionView.UnregisterAction(Initialize, SidebarSelectionViewEventType.Initialize);
            _sidebarSelectionView.UnregisterAction(OnBeforeEnter, SidebarSelectionViewEventType.OnBeforeEnter);
            _sidebarSelectionView.UnregisterAction(OnAfterRelease, SidebarSelectionViewEventType.OnAfterExit, 
                SidebarSelectionViewEventType.OnAfterExitImmediate);
        }

        private void Update()
        {
            var selectableTransform = _sidebarSelectionView.SelectableGo.transform;
            var selectablePosition = selectableTransform.position;

            WriteNumberAsSubstring(_positionX, selectablePosition.x);
            WriteNumberAsSubstring(_positionY, selectablePosition.y);
            WriteNumberAsSubstring(_positionZ, selectablePosition.z);
        
            var selectableRotation = selectableTransform.rotation.eulerAngles;
            WriteNumberAsSubstring(_rotationX, selectableRotation.x);
            WriteNumberAsSubstring(_rotationY, selectableRotation.y);
            WriteNumberAsSubstring(_rotationZ, selectableRotation.z);
        
            var selectableScale = selectableTransform.lossyScale;
            WriteNumberAsSubstring(_scaleX, selectableScale.x);
            WriteNumberAsSubstring(_scaleY, selectableScale.y);
            WriteNumberAsSubstring(_scaleZ, selectableScale.z);
            
            if (_instantiatedGizmo != null)
            {
                _instantiatedGizmo.transform.position = _sidebarSelectionView.SelectableGo.transform.position;

                if (_rotationSwapInteractionToggle.isOn)
                {
                    _instantiatedGizmo.transform.rotation = _sidebarSelectionView.SelectableGo.transform.rotation;
                }
            }
        }
        
        #endregion

        #region Private Methods
        
        private void Initialize()
        {
            _sidebarSelectionView.SelectionViewStackMachine.PushStackElement(_sidebarSelectionView);
        }

        private void OnBeforeEnter()
        {
            InstantiateGizmo();
            InstantiateTransformInteractable();
            
            NetworkInteractionSelectable.SetSelectionBlocked(true);
            
            _positionSwapInteractionToggle.onValueChanged.AddListener(_ => SetupInteractable<XRMoveInteractable>(Quaternion.identity));
            _rotationSwapInteractionToggle.onValueChanged.AddListener(_ => SetupInteractable<XRRotateInteractable>(_sidebarSelectionView.SelectableGo.transform.rotation));
            _scaleSwapInteractionToggle.onValueChanged.AddListener(_ => SetupInteractable<XRScaleInteractable>(_sidebarSelectionView.SelectableGo.transform.rotation));
        }
        
        private void OnAfterRelease()
        {
            DestroyTransformInteractable();
            
            NetworkInteractionSelectable.SetSelectionBlocked(false);
            
            _positionSwapInteractionToggle.onValueChanged.RemoveListener(_ => SetupInteractable<XRMoveInteractable>(Quaternion.identity));
            _rotationSwapInteractionToggle.onValueChanged.RemoveListener(_ => SetupInteractable<XRRotateInteractable>(_sidebarSelectionView.SelectableGo.transform.rotation));
            _scaleSwapInteractionToggle.onValueChanged.RemoveListener(_ => SetupInteractable<XRScaleInteractable>(_sidebarSelectionView.SelectableGo.transform.rotation));

            DestroyGizmo();
        }
        
        private void WriteNumberAsSubstring(TMP_Text tmpText, float number)
        {
            string numberAsString = number.ToString(CultureInfo.CurrentCulture); 
            tmpText.text = numberAsString.Substring(0, Math.Min(numberAsString.Length, _numberDigitAmount));
        }
        
        private void InstantiateTransformInteractable()
        {
            switch (_entrySelectionType)
            {
                case SelectionType.Move:
                    _positionSwapInteractionToggle.isOn = true;
                    SetupInteractable<XRMoveInteractable>(Quaternion.identity);
                    break;
                case SelectionType.Rotate:
                    _rotationSwapInteractionToggle.isOn = true;
                    SetupInteractable<XRRotateInteractable>(_sidebarSelectionView.SelectableGo.transform.rotation);
                    break;
                case SelectionType.Scale:
                    _scaleSwapInteractionToggle.isOn = true;
                    SetupInteractable<XRScaleInteractable>(_sidebarSelectionView.SelectableGo.transform.rotation);
                    break;
                case SelectionType.Grab:
                    SetupInteractable<XRGrabInteractable>(Quaternion.identity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void InstantiateGizmo()
        {
            Vector3 spawnPosition = _sidebarSelectionView.SelectableGo.transform.position;

            _instantiatedGizmo = Instantiate(_gizmo, spawnPosition, Quaternion.identity);
        }
        
        private void DestroyGizmo()
        {
            Destroy(_instantiatedGizmo);
            _instantiatedGizmo = null;
        }

        private void SetupInteractable<T>(Quaternion newRotation) where T : XRTransformerInteractable
        {
            if (_interactable != null)
            {
                DestroyTransformInteractable();
            }
            
            _interactable = _sidebarSelectionView.SelectableGo.gameObject.AddComponent<T>();
            _interactable.InteractorGroup = _sidebarSelectionView.InteractorGroup;
            _instantiatedGizmo.transform.rotation = newRotation;
        }

        private void DestroyTransformInteractable()
        {
            Destroy(_interactable);
            _interactable = null;
        }
        
        #endregion
    }
}
