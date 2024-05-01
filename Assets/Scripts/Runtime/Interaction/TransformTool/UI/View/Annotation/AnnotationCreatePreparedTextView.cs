using UnityEngine;
using VENTUS.Interaction.TransformTool.Core.View;
using VENTUS.Interaction.TransformTool.UI.View.PreparedText;

namespace VENTUS.Interaction.TransformTool.UI.View.Annotation
{
    public class AnnotationCreatePreparedTextView : BasePreparedTextSpawner
    {
        [Header("Annotation")]
        [SerializeField] private SidebarSelectionView _sidebarSelectionView;

        protected override void Awake()
        {
            base.Awake();
            _sidebarSelectionView.RegisterAction(Unfocus, SidebarSelectionViewEventType.OnBeforeRelease, 
                SidebarSelectionViewEventType.OnBeforeExit, SidebarSelectionViewEventType.OnBeforeExitImmediate);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _sidebarSelectionView.UnregisterAction(Unfocus, SidebarSelectionViewEventType.OnBeforeRelease, 
                SidebarSelectionViewEventType.OnBeforeExit, SidebarSelectionViewEventType.OnBeforeExitImmediate);
        }
    }
}
