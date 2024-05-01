using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VENTUS.Interaction.TransformTool.Annotation;
using VENTUS.Interaction.TransformTool.Core.View;

namespace VENTUS.Interaction.TransformTool.UI.View.Annotation
{
    public class AnnotationEditSelectionView : MonoBehaviour
    {
        [SerializeField] private SidebarSelectionView _sidebarSelectionView;
        
        [Header("Annotation")]
        [SerializeField] private AnnotationEditSelectionViewElement _elementPrefab;
        [SerializeField] private Transform _elementPrefabInstantiationParent;
        [SerializeField] private AnnotationControllerRuntimeSet _annotationControllerRuntimeSet;
        [SerializeField] private AnnotationView _annotationEditView;
        
        private readonly List<AnnotationController> _annotationControllers = new();
        private readonly List<(AnnotationController, AnnotationEditSelectionViewElement)> _selectionViewElements = new();

        protected void Awake()
        {
            _sidebarSelectionView.RegisterAction(Initialize, 
                SidebarSelectionViewEventType.Initialize);
            
            _sidebarSelectionView.RegisterAction(Exit, 
                SidebarSelectionViewEventType.OnAfterExit, 
                SidebarSelectionViewEventType.OnAfterExitImmediate);
        }

        private void OnDestroy()
        {
            _sidebarSelectionView.UnregisterAction(Initialize, 
                SidebarSelectionViewEventType.Initialize);
            
            _sidebarSelectionView.UnregisterAction(Exit, 
                SidebarSelectionViewEventType.OnAfterExit, 
                SidebarSelectionViewEventType.OnAfterExitImmediate);
        }

        private void Initialize()
        {
            _sidebarSelectionView.SelectionViewStackMachine.PushStackElement(_sidebarSelectionView);
            
            UpdateAnnotationControllers();
        }

        private void UpdateAnnotationControllers()
        {
            _annotationControllers.Clear();
            foreach (var childToParentTransform in _annotationControllerRuntimeSet.GetItems())
            {
                if (childToParentTransform.SelectableGo == _sidebarSelectionView.SelectableGo)
                {
                    _annotationControllers.Add(childToParentTransform);
                }
            }
        }

        private void Exit()
        {
            RemoveAll();
        }

        private void Update()
        {
            UpdateAnnotationControllers();
            List<AnnotationController> selectableAnnotationLookup = _annotationControllers.ToList();
            
            for (var index = _selectionViewElements.Count - 1; index >= 0; index--)
            {
                var annotationEditSelectionViewElement = _selectionViewElements[index];
                
                //remove from lookup, if annotation controller doesnt exists anymore
                if (!selectableAnnotationLookup.Contains(annotationEditSelectionViewElement.Item1))
                {
                    RemoveAnnotationController(annotationEditSelectionViewElement);
                }
                else //remove it from the tracking list if it is already registered
                {
                    selectableAnnotationLookup.Remove(annotationEditSelectionViewElement.Item1);
                }
            }
            
            //add all leftover elements
            foreach (var annotationController in selectableAnnotationLookup)
            {
                AddElement(annotationController);
            }
        }
        
        private void AddElement(AnnotationController annotationController)
        {
            AnnotationEditSelectionViewElement annotationEditSelectionViewElement = Instantiate(_elementPrefab, _elementPrefabInstantiationParent);
            annotationEditSelectionViewElement.Initialize(_sidebarSelectionView.InteractorGroup, _sidebarSelectionView.SelectableGo, _sidebarSelectionView.SelectionViewStackMachine);
            annotationEditSelectionViewElement.ApplyAnnotationData(annotationController);
            _selectionViewElements.Add((annotationController, annotationEditSelectionViewElement));
            annotationController.SetViewToSpawn(_annotationEditView);
        }
        
        private void RemoveAnnotationController((AnnotationController, AnnotationEditSelectionViewElement) listEntry)
        {
            if (_selectionViewElements.Remove(listEntry))
            {
                listEntry.Item1.SetDefaultViewToSpawn();
                Destroy(listEntry.Item2.gameObject);
            }
        }

        private void RemoveAll()
        {
            foreach (var selectionViewElement in _selectionViewElements)
            {
                selectionViewElement.Item1.SetDefaultViewToSpawn();
                Destroy(selectionViewElement.Item2.gameObject);
            }
        }
    }
}
