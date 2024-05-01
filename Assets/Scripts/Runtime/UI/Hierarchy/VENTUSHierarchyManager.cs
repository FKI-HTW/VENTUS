using CENTIS.UnityHierarchyView;
using UnityEngine;

namespace VENTUS.UI.Hierarchy
{
	public class VENTUSHierarchyManager : HierarchyManager
	{
		[SerializeField] private GameObject _hierarchyUI;

		public override void OpenHierarchyView(Transform transform, Transform initial)
		{
			if (_hierarchyUI.activeSelf)
				CloseHierarchyView();
			base.OpenHierarchyView(transform, initial);
			
			_hierarchyUI.transform.position = transform.position + Vector3.right * 5;
			_hierarchyUI.SetActive(true);
		}

		public override void CloseHierarchyView()
		{
			base.CloseHierarchyView();
			_hierarchyUI.SetActive(false);
		}
	}
}
