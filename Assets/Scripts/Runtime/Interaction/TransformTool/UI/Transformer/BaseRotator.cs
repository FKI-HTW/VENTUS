using UnityEngine;
using VENTUS.Interaction.TransformTool.UI.View;

namespace VENTUS.Interaction.TransformTool.UI.Transformer
{
    public abstract class BaseRotator : ScriptableObject
    {
        public abstract void UpdateRotation(Transform transformToRotate, GameObject selectableGo);
    }
}
