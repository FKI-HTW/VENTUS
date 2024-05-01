using System.Collections;

namespace VENTUS.Interaction.TransformTool.Core.View
{
    public interface ICoroutineStackElement
    {
        /// <summary>
        /// Will be called when this element gets Pushed to the stack
        /// </summary>
        /// <returns></returns>
        public IEnumerator OnEnter();
    
        /// <summary>
        /// Will be called when another element gets Popped from the stack (on top of this element) and this element gets active
        /// </summary>
        /// <returns></returns>
        public IEnumerator OnFocus();
    
        /// <summary>
        /// Will be called when another element gets Pushed to the stack (on top of this element) and this element gets inactive
        /// </summary>
        /// <returns></returns>
        public IEnumerator OnRelease();
    
        /// <summary>
        /// Will be called when this element gets Popped from the stack
        /// </summary>
        /// <returns></returns>
        public IEnumerator OnExit();

        /// <summary>
        /// Will be called when this element gets Popped from the stack. This will only happen for element that are inactive during the ClearStack call.
        /// </summary>
        public void OnExitImmediate();
    }
}
