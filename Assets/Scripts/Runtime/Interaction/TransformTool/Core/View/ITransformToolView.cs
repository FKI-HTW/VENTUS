using UnityEngine;

namespace VENTUS.Interaction.TransformTool.Core.View
{
    public interface ITransformToolView : ICoroutineStackElement
    {
        public MonoBehaviour MonoBehaviour { get; }
    }
}
