using CENTIS.UnityHierarchyView;
using UnityEngine;
using VENTUS.Interaction.TransformTool.Core.View;
using VENTUS.Interaction.TransformTool.UI.Clickable;
using VENTUS.ModelImporter;

namespace VENTUS.UI.Hierarchy
{
	public class VENTUSSelectableHierarchyManager : HierarchyManager
	{
		[SerializeField] private SidebarSelectionView _sidebarSelectionView;
		[SerializeField] private ModelDestroyClickable _modelDestroyClickable;

		private void Awake()
		{
			_sidebarSelectionView.RegisterAction(
				() => _sidebarSelectionView.SelectionViewStackMachine.PushStackElement(_sidebarSelectionView), 
				SidebarSelectionViewEventType.Initialize);
			
			_sidebarSelectionView.RegisterAction(
				() => OpenHierarchyView(SearchHighestModelParent(), _sidebarSelectionView.SelectableGo.transform), 
				SidebarSelectionViewEventType.OnBeforeEnter);
			
			_sidebarSelectionView.RegisterAction(CloseHierarchyView, SidebarSelectionViewEventType.OnAfterExit, 
				SidebarSelectionViewEventType.OnAfterExitImmediate);
		}

		private void Start()
		{
			_modelDestroyClickable.ObjectToDestroy = SearchHighestModelParent().gameObject;
		}

		private void OnDestroy()
		{
			_sidebarSelectionView.UnregisterAction(
				() => _sidebarSelectionView.SelectionViewStackMachine.PushStackElement(_sidebarSelectionView), 
				SidebarSelectionViewEventType.Initialize);
			
			_sidebarSelectionView.UnregisterAction(
				() => OpenHierarchyView(SearchHighestModelParent(), _sidebarSelectionView.SelectableGo.transform), 
				SidebarSelectionViewEventType.OnBeforeEnter);
			
			_sidebarSelectionView.UnregisterAction(CloseHierarchyView, SidebarSelectionViewEventType.OnAfterExit, 
				SidebarSelectionViewEventType.OnAfterExitImmediate);
		}

		private Transform SearchHighestModelParent()
		{
			Transform current = _sidebarSelectionView.SelectableGo.transform;
			
			while (current.parent != null && current.parent.TryGetComponent(out NetworkedModelObject _))
			{
				current = current.parent;
			}

			return current;
		}
	}
}
