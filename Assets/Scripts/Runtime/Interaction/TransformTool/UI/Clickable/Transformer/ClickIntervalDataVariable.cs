using System;
using System.Collections.Generic;
using UnityEngine;
using VENTUS.DataStructures.Variables;

namespace VENTUS.Interaction.TransformTool.UI.Clickable.Transformer
{
    [CreateAssetMenu(fileName = "new ClickIntervalDataVariable", menuName = "VENTUS/TransformTool/ClickIntervalData Variable")]
    public class ClickIntervalDataVariable : AbstractVariable<List<ClickIntervalData>>
    {
    }

    [Serializable]
    public class ClickIntervalData
    {
        public float durationToPrevious;
        public float clickStrength;
        public bool triggerOnce;
    }
}