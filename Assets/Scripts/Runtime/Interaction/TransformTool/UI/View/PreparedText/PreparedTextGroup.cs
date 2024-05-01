using UnityEngine;
using VENTUS.DataStructures.RuntimeSet;

namespace VENTUS.Interaction.TransformTool.UI.View.PreparedText
{
    [CreateAssetMenu(fileName = "new ExampleTextElement", menuName = "VENTUS/TransformTool/ExampleTextGroup")]
    public class PreparedTextGroup : RuntimeSet<PreparedTextElement>
    {
        [SerializeField] private string _groupName;

        public string GroupName => _groupName;
    }
}
