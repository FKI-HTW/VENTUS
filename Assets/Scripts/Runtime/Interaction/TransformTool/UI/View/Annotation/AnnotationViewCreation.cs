using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VENTUS.Controlling;
using VENTUS.Interaction.Core.Controller;
using VENTUS.Interaction.TransformTool.Annotation;
using VENTUS.Interaction.TransformTool.Core.View;
using VENTUS.Networking.BroadcastInstaller;
using VENTUS.UI.Input;

namespace VENTUS.Interaction.TransformTool.UI.View.Annotation
{
    public class AnnotationViewCreation : MonoBehaviour
    {
        public static event Action<AnnotationViewCreation, GameObject> OnEnterAnnotationPlacement;
        public static event Action OnExitAnnotationPlacement;
        
        private enum AnnotationCreationState { None, DataInput, RayInteractionPlacement }

        [SerializeField] private float _disableTime;
        
        [Header("Data")]
        [SerializeField] private SidebarSelectionView _relatedSidebarSelectionView;
        [SerializeField] private InformationView _informationViewPrefab;
        [SerializeField] private AnnotationController _annotationControllerPrefab;
        
        [Header("Data")]
        [SerializeField] private GameObject _dataInput;
        [SerializeField] private TMP_InputField _titleInput;
        [SerializeField] private TMP_InputField _descriptionInput;
        [SerializeField] private Button _confirmButton;

        [Header("Placement")]
        [SerializeField] private GameObject _rayInteractionPlacementText;
        [SerializeField] private GameObject _annotationVisual;
        [SerializeField] private float _distance;
        [SerializeField] private LayerMask _raycastMask;

        private AnnotationCreationState _annotationCreationState;
        private KeyboardManager _keyboardManager;
        private RectTransform _titleRect;
        private RectTransform _descriptionRect;

        private bool _hasSpawnedAnnotation;
        private Vector3 _spawnPosition;

        protected void Awake()
        {
            //sidebar
            _relatedSidebarSelectionView.RegisterAction(Initialize, SidebarSelectionViewEventType.Initialize);
            _relatedSidebarSelectionView.RegisterAction(OnExit, SidebarSelectionViewEventType.OnBeforeExit);
            _relatedSidebarSelectionView.RegisterAction(OnExitImmediate, SidebarSelectionViewEventType.OnBeforeExitImmediate);
            
            //input
            _keyboardManager = GameObject.FindGameObjectWithTag("KeyboardManager").GetComponent<KeyboardManager>();
            if (_keyboardManager != null)
            {
                _titleInput.onSelect.AddListener(x => InitTitleInputKeyboard());
                _descriptionInput.onSelect.AddListener(x => InitDescriptionInputKeyboard());
            }
            _titleRect = _titleInput.GetComponent<RectTransform>();
            _descriptionRect = _descriptionInput.GetComponent<RectTransform>();
            
            //placement
            _confirmButton.onClick.AddListener(TryEnterRayInteractionPlacement);
        }

        private void OnDestroy()
        {
            //sidebar
            _relatedSidebarSelectionView.UnregisterAction(Initialize, SidebarSelectionViewEventType.Initialize);
            _relatedSidebarSelectionView.UnregisterAction(OnExit, SidebarSelectionViewEventType.OnBeforeExit);
            _relatedSidebarSelectionView.UnregisterAction(OnExitImmediate, SidebarSelectionViewEventType.OnBeforeExitImmediate);
            
            //input
            if (_keyboardManager != null)
            {
                _titleInput.onSelect.RemoveListener(x => InitTitleInputKeyboard());
                _descriptionInput.onSelect.RemoveListener(x => InitDescriptionInputKeyboard());
            }
            
            //placement
            _confirmButton.onClick.RemoveListener(TryEnterRayInteractionPlacement);
        }

        #region Private Methods
        
        private void Initialize()
        {
            _relatedSidebarSelectionView.SelectionViewStackMachine.PushStackElement(_relatedSidebarSelectionView);
            EnterDataInput();
            _rayInteractionPlacementText.SetActive(false);
        }

        private void OnExit()
        {
            if (_annotationCreationState == AnnotationCreationState.RayInteractionPlacement)
            {
                ExitRayInteractionPlacement();

                if (_hasSpawnedAnnotation)
                {
                    //move to position
                    transform.DOMove(_spawnPosition, _disableTime).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        
                    //set scale
                    transform.DOScale(Vector3.zero, _disableTime).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
                }
            }
        }

        private void OnExitImmediate()
        {
            if (_annotationCreationState == AnnotationCreationState.RayInteractionPlacement)
            {
                ExitRayInteractionPlacement();
            }
        }
        
        private void InitTitleInputKeyboard()
        {
            if (SceneManager.IsInXRMode)
            {
                _keyboardManager.GetKeyboardInput(
                    KeyboardManager.KeyboardTypes.Small,
                    connectedInputField: _titleInput,
                    onInputWasUpdated: _ => _titleRect.sizeDelta = new(
                        _titleRect.sizeDelta.x, 
                        _titleInput.textComponent.preferredHeight
                    )
                );
            }
        }

        private void InitDescriptionInputKeyboard()
        {
            if (SceneManager.IsInXRMode)
            {
                _keyboardManager.GetKeyboardInput(
                    KeyboardManager.KeyboardTypes.Small,
                    connectedInputField: _descriptionInput,
                    onInputWasUpdated: _ => _descriptionRect.sizeDelta = new(
                        _descriptionRect.sizeDelta.x, 
                        _descriptionInput.textComponent.preferredHeight
                    )
                );
            }
        }
        
        private void TryEnterRayInteractionPlacement()
        {
            _keyboardManager.CloseKeyboard();
            
            if (!IsValidTextInput(out string errorMessage))
            {
                InstantiateInformationView(errorMessage);
                return;
            }
            
            ExitDataInput();
            EnterRayInteractionPlacement();
        }
        
        private void InstantiateInformationView(string errorMessage)
        {
            InformationView instantiatedView = Instantiate(_informationViewPrefab);
            
            foreach (var baseSelectionElement in instantiatedView.GetComponentsInChildren<BaseSelectionElement>())
            {
                baseSelectionElement.Initialize(_relatedSidebarSelectionView.InteractorGroup, 
                    _relatedSidebarSelectionView.SelectableGo, 
                    _relatedSidebarSelectionView.SelectionViewStackMachine);
            }
            
            instantiatedView.AddInformationContext(errorMessage, () => _relatedSidebarSelectionView.SelectionViewStackMachine.PopStackElement());
        }

        private void EnterDataInput()
        {
            _annotationCreationState = AnnotationCreationState.DataInput;
            _dataInput.SetActive(true);
        }

        private void ExitDataInput()
        {
            _annotationCreationState = AnnotationCreationState.None;
            _dataInput.SetActive(false);
        }
        
        private void EnterRayInteractionPlacement()
        {
            _annotationCreationState = AnnotationCreationState.RayInteractionPlacement;
            _rayInteractionPlacementText.SetActive(true);
            
            InteractionController.InteractorDown += InstantiateMinimizedAnnotation;
            OnEnterAnnotationPlacement?.Invoke(this, _annotationVisual);
        }

        private void ExitRayInteractionPlacement()
        {
            _annotationCreationState = AnnotationCreationState.None;
            _rayInteractionPlacementText.SetActive(false);
            
            InteractionController.InteractorDown -= InstantiateMinimizedAnnotation;
            OnExitAnnotationPlacement?.Invoke();
        }
        
        private void InstantiateMinimizedAnnotation(InteractorGroup interactorGroup)
        {
            if (!IsValidPlacementRaycast(interactorGroup.EnabledInteractionInstances.First().Origin, out RaycastHit raycastHit, out string errorMessage))
            {
                InstantiateInformationView(errorMessage);
                return;
            }
            
            if (!IsValidObject(raycastHit.transform, out errorMessage))
            {
                InstantiateInformationView(errorMessage);
                return;
            }

            SpawnAnnotationBroadcastInstaller.BroadcastSpawnToServer(_annotationControllerPrefab,
                _relatedSidebarSelectionView.SelectableGo.transform,
                raycastHit.point,
                Quaternion.LookRotation(raycastHit.normal),
                _titleInput.text,
                _descriptionInput.text);
            
            //no need to apply the current snapshot here, because it already got set active by spawning
            _relatedSidebarSelectionView.SelectionViewStackMachine.PopStackElement();
        }
        
        #endregion
        
        #region Public Methods
        
        public void NavigateBack()
        {
            switch (_annotationCreationState)
            {
                case AnnotationCreationState.None:
                    break;
                case AnnotationCreationState.DataInput:
                    _relatedSidebarSelectionView.SelectionViewStackMachine.PopStackElement();
                    break;
                case AnnotationCreationState.RayInteractionPlacement:
                    ExitRayInteractionPlacement();
                    EnterDataInput();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public bool IsValidTextInput(out string errorMessage)
        {
            errorMessage = "";
            if (_titleInput.text == string.Empty && _descriptionInput.text == string.Empty)
            {
                errorMessage = "You have enter text to either the title or the description!";
                return false;
            }
            
            return true;
        }
        
        public bool IsValidPlacementRaycast(Transform originTransform, out RaycastHit raycastHit, out string errorMessage)
        {
            raycastHit = default;
            errorMessage = "";
            if (!Physics.Raycast(originTransform.position, originTransform.forward, out raycastHit, _distance, _raycastMask))
            {
                errorMessage = "You have to hover and add the annotation to the currently selected object!";
                return false;
            }
            
            return true;
        }

        public bool IsValidObject(Transform comparedObject, out string errorMessage)
        {
            errorMessage = "";
            if (comparedObject == _relatedSidebarSelectionView.SelectableGo.transform) return true;
            
            errorMessage = "You have to hover and add the annotation to the currently selected object!";
            return false;
        }
        
        #endregion
    }
}
