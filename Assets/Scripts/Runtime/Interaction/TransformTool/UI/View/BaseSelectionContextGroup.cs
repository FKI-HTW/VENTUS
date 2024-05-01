using System.Collections.Generic;
using UnityEngine;
using VENTUS.Interaction.TransformTool.Core.View;

namespace VENTUS.Interaction.TransformTool.UI.View
{
    [CreateAssetMenu(fileName = "new BaseSelectionContextGroup", menuName = "VENTUS/TransformTool/Base Selection Context Group")]
    public class BaseSelectionContextGroup : ScriptableObject
    {
        public List<BaseSelectionElement> additionalSelectionPrefabs;
    }
}
