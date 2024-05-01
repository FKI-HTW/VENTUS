using UnityEngine;
using VENTUS.Interaction.TransformTool.Core.View;

namespace VENTUS.Interaction.TransformTool.UI.View
{
    public class SelectionViewStackMachineElement : MonoBehaviour
    {
        [SerializeField] private SidebarSelectionView _sidebarSelectionView;

        private void Awake()
        {
            _sidebarSelectionView.RegisterAction(
                () => _sidebarSelectionView.SelectionViewStackMachine.PushStackElement(_sidebarSelectionView), 
                SidebarSelectionViewEventType.Initialize);
        }

        private void OnDestroy()
        {
            _sidebarSelectionView.UnregisterAction(
                () => _sidebarSelectionView.SelectionViewStackMachine.PushStackElement(_sidebarSelectionView), 
                SidebarSelectionViewEventType.Initialize);
        }
    }
}
