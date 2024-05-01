using System.Collections;
using UnityEngine;

namespace VENTUS.Interaction.TransformTool.Core.View
{
    public class EmptyCoroutineStackElement : ITransformToolView
    {
        private readonly MonoBehaviour _monoBehaviour;
        
        /// <summary>
        /// A stack element from a SelectionViewStackMachine always needs a dependant monoBehaviour, even if it is empty.
        /// Used for adding a view
        /// </summary>
        /// <param name="monoBehaviour"></param>
        public EmptyCoroutineStackElement(MonoBehaviour monoBehaviour)
        {
            _monoBehaviour = monoBehaviour;
        }

        public MonoBehaviour MonoBehaviour
        {
            get
            {
                Debug.LogWarning($"Returned the fallback MonoBehaviour of an {typeof(EmptyCoroutineStackElement)}!");
                return _monoBehaviour;
            }
        }

        public IEnumerator OnEnter()
        {
            yield return null;
        }

        public IEnumerator OnFocus()
        {
            yield return null;
        }

        public IEnumerator OnRelease()
        {
            yield return null;
        }

        public IEnumerator OnExit()
        {
            yield return null;
        }

        public void OnExitImmediate() { }
    }
}
