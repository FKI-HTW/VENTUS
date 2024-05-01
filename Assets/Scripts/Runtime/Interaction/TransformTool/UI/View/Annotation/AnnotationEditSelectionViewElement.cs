using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VENTUS.Interaction.TransformTool.Annotation;
using VENTUS.Interaction.TransformTool.Core.View;
using VENTUS.Networking.BroadcastInstaller;

namespace VENTUS.Interaction.TransformTool.UI.View.Annotation
{
    public class AnnotationEditSelectionViewElement : BaseSelectionElement
    {
        [Header("AnnotationEditElement")]
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Button _openButton;
        
        [Header("Destroy")]
        [SerializeField] private ConfirmView _confirmViewPrefab;
        [SerializeField] private Button _destroyButton;
        [SerializeField] private string _destroyText;

        private AnnotationController _annotationController;

        public void ApplyAnnotationData(AnnotationController annotationController)
        {
            _annotationController = annotationController;
            
            _title.text = _annotationController.Title;
            
            _openButton.onClick.AddListener(_annotationController.Open);
            _destroyButton.onClick.AddListener(DisposeAnnotationController);
        }

        private void OnDestroy()
        {
            _openButton.onClick.RemoveListener(_annotationController.Open);
            _destroyButton.onClick.RemoveListener(DisposeAnnotationController);
        }

        private void DisposeAnnotationController()
        {
            ConfirmView instantiatedView = Instantiate(_confirmViewPrefab);
            
            foreach (var baseSelectionElement in instantiatedView.GetComponentsInChildren<BaseSelectionElement>())
            {
                baseSelectionElement.Initialize(InteractorGroup, SelectableGo, SelectionViewStackMachine);
            }
            
            instantiatedView.AddConfirmContext(_destroyText, () =>
            {
                GameObject[] objectsToDestroy = { _annotationController.gameObject };
                ActiveStateBroadcastInstaller.BroadcastActiveStateToServer(objectsToDestroy, false, true);
                SelectionViewStackMachine.PopStackElement();
            }, () => SelectionViewStackMachine.PopStackElement());
        }
    }
}
