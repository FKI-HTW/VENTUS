using UnityEngine;

namespace VENTUS.Interaction.TransformTool.UI.Transformer
{
    public abstract class BasePositioner : ScriptableObject
    {
        public abstract Vector3 GetUpdatedPosition(GameObject selectableGo);
    }
}
