using UnityEngine;
using VENTUS.Controlling;
using VENTUS.Interaction.TransformTool.UI.View;

namespace VENTUS.Interaction.TransformTool.UI.Transformer
{
    [CreateAssetMenu(fileName = "new BillboardRotator", menuName = "VENTUS/TransformTool/Rotator/Billboard")]
    public class BillboardRotator : BaseRotator
    {
        [SerializeField] private bool _mirror;
    
        public override void UpdateRotation(Transform transformToRotate, GameObject selectableGo)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;
        
            BillboardUtility.UpdateRotation(transformToRotate, mainCamera.transform, _mirror);
        }
    }
}
