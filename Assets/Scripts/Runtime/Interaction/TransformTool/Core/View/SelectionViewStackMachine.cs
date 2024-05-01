using System.Collections;
using UnityEngine;
using VENTUS.Interaction.TransformTool.Core.Focus;
using VENTUS.Interaction.TransformTool.Core.Selection;

namespace VENTUS.Interaction.TransformTool.Core.View
{
    public class SelectionViewStackMachine : CoroutineStackMachine<ITransformToolView>
    {
        private readonly NetworkInteractionTransformSelectable _networkInteractionSelectable;
        private readonly float _disposeTime;
        
        protected override float InitializationTime { get; }

        public SelectionViewStackMachine(NetworkInteractionTransformSelectable networkInteractionSelectable, 
            float initializationTime = 0f, float disposeTime = 0f)
        {
            _networkInteractionSelectable = networkInteractionSelectable;
            _disposeTime = disposeTime;
            InitializationTime = initializationTime;
        }

        protected override void OnBeforeInitialize() { }

        protected override void OnAfterInitialize() { }

        protected override IEnumerator OnStackEmpty()
        {
            yield return new WaitForSeconds(_disposeTime);

            // Given the mutual ability of the SelectionFocusSystem and the SelectionViewStackMachine to exit each other,
            // it's essential to ensure that each one performs this operation exactly once.
            if (_networkInteractionSelectable.IsSelected)
            {
                SelectionFocusSystem.ExitSelection(_networkInteractionSelectable.GetType());
            }
            
            //Ownership needs to be released after the coroutine operations to clear the stack
            _networkInteractionSelectable.ReleaseOwnership();
        }
    }
}
