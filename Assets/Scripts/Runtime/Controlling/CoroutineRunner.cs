using System.Collections;
using UnityEngine;

namespace VENTUS.Controlling
{ 
    /// <summary>
    ///     From: https://gist.github.com/jonHuffman/c252e4e8f61399383ff2eef1b44e130e
    ///     Allows for a coroutine to be run from objects that do not extend Monobehaviour
    /// </summary>
    /// <remarks>
    ///     Only use a coroutine in a non-monobehaviour if aboslutely needed as it will place a Unity dependancy in your code
    /// </remarks>
    public static class CoroutineRunner
    {
        private const string ROUNTINE_RUNNER_NAME = "CoroutineRunner";

        public delegate void DelayedFunction();

        private static GameObject _handlerParent;
        private static CR _coroutineHandler;

        private static DelayedFunction _delayedFunction;

        /// <summary>
        ///     Allows for non-monobehaviours to run coroutines
        /// </summary>
        /// <param name="method">Coroutine must be passed in as an IEnumerator</param>
        public static Coroutine RunCoroutine(this IEnumerator method)
        {
            if (_handlerParent == null) CreateRunner();
            return _coroutineHandler.StartCoroutine(method);
        }

        /// <summary>
        ///     Allows non-monobehaviours to stop coroutines that were started using CoroutineRunner
        /// </summary>
        /// <param name="method">Coroutine must be passed in as an IEnumerator</param>
        public static void StopCoroutine(this IEnumerator method)
        {
            if (_handlerParent == null)
            {
                  Debug.LogError("Coroutine Runner : There is no active coroutine object.");
                  return;
            }

            _coroutineHandler.StopCoroutine(method);
        }

        /// <summary>
        ///     Creates the actual Coroutine Runner object. This object will be what owns and executes the Coroutines.
        /// </summary>
        private static void CreateRunner()
        {
              _handlerParent = new GameObject();
              _coroutineHandler = _handlerParent.AddComponent<CR>();
              _coroutineHandler.name = ROUNTINE_RUNNER_NAME;
              Object.DontDestroyOnLoad(_handlerParent);
        }
    }

    /// <summary>
    ///     The MonoBehaviour that owns and executes the Coroutines that are owned by the Coroutine Runner.
    /// </summary>
    /// <remarks>
    ///     By sharing a .cs file that does not share a name with this class we effectively hide the Monobehaviour from Unity.
    ///     This prevents someone from accidentally stumbling upon the component and trying to add it manually.
    /// </remarks>
    public class CR : MonoBehaviour
    {
          /// <summary>
          ///     Stops all active coroutines when destroyed
          /// </summary>
          private void OnDestroy()
          {
                StopAllCoroutines();
          }
    }
}
