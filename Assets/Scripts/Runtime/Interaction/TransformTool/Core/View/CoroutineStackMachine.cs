using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VENTUS.Controlling;

namespace VENTUS.Interaction.TransformTool.Core.View
{
    /// <summary>
    /// This is an abstract class designed to manage a stack of coroutine-based elements of type T.
    /// This class is particularly useful in scenarios where you need to handle a dynamic sequence of operations or states.
    /// </summary>
    /// <typeparam name="T">Type of the stack. It must inherit of ICoroutineStackElement</typeparam>
    public abstract class CoroutineStackMachine<T> where T : ICoroutineStackElement
    {
        #region Fields
        
        private readonly MonoBehaviour _coroutineRunner;
        private readonly Stack<T> _selectionViewStack = new();
        private readonly Queue<Action> _requestQueue = new();
    
        public T CurrentElement => _selectionViewStack.Peek();
        public IEnumerable<T> Enumerable => _selectionViewStack;
        public bool HasStack { get; private set; }
        public bool IsCoroutineRunning { get; private set; }
        protected abstract float InitializationTime { get; }
        
        #endregion

        #region Public Methods
    
        /// <summary>
        /// Add a new element to the stack, set it as active (calls: OnEnter) and disable the previous (calls: OnRelease).
        /// If there is a coroutine running when you call this method, the new element will be added to a queue to be called next.
        /// </summary>
        public void PushStackElement(T coroutineStackElement)
        {
            ProcessEnqueue(() => InternalPushStackElement(coroutineStackElement).RunCoroutine());
        }
    
        /// <summary>
        /// Remove the current element from the stack and disable it (calls: OnExit). If there is still another element left, set it active (calls: OnFocus). Else dispose the current stack.
        /// If there is a coroutine running when you call this method, the new element will be added to a queue to be called next.
        /// </summary>
        public void PopStackElement()
        {
            ProcessEnqueue(() => InternalPopStackElement().RunCoroutine());
        }
    
        /// <summary>
        /// Removes all elements from the current stack. The Current element will be disabled by coroutine (calls: OnExit). Everything else instantaneously (calls: OnExitImmediate).
        /// </summary>
        /// <param name="onStackCleared">Action to be performed, after the stack was cleared.</param>
        public void ClearStack(Action onStackCleared = null)
        {
            ProcessEnqueue(() => InternalClearStack(onStackCleared).RunCoroutine());
        }
    
        #endregion
    
        #region Private Methods
    
        /// <summary>
        /// In case a request was added during a coroutine, we need to save and start it at the end of the coroutine
        /// </summary>
        /// <param name="queueElement">Your request</param>
        private void ProcessEnqueue(Action queueElement)
        {
            if (IsCoroutineRunning)
            {
                _requestQueue.Enqueue(queueElement.Invoke);
            }
            else
            {
                queueElement.Invoke();
            }
        }
    
        /// <summary>
        /// Continue with the current element in the added requestStack queue
        /// </summary>
        private void ProcessDequeue()
        {
            if (_requestQueue.Count != 0)
            {
                _requestQueue.Dequeue().Invoke();
            }
        }

        private IEnumerator InternalPushStackElement(T coroutineStackElement)
        {
            IsCoroutineRunning = true;
        
            //Initialize
            if (_selectionViewStack.Count == 0)
            {
                yield return OnInitialize();
            }
        
            //Set current as inactive
            if (_selectionViewStack.Count != 0)
            {
                yield return CurrentElement.OnRelease();
            }
        
            //Set new as active
            _selectionViewStack.Push(coroutineStackElement);
            yield return coroutineStackElement.OnEnter();
        
            IsCoroutineRunning = false;
            ProcessDequeue();
        }
    
        private IEnumerator InternalPopStackElement()
        {
            IsCoroutineRunning = true;
        
            //Set current as inactive
            if (_selectionViewStack.Count != 0)
            {
                yield return CurrentElement.OnExit();
                _selectionViewStack.Pop();
            }

            //Activate next if there is still one, else: dispose
            if (_selectionViewStack.Count != 0)
            {
                yield return CurrentElement.OnFocus();
                IsCoroutineRunning = false;
                ProcessDequeue();
            }
            else
            {
                HasStack = false;
                _requestQueue.Clear();
                yield return OnStackEmpty();
                IsCoroutineRunning = false;
            }
        }

        private IEnumerator InternalClearStack(Action onStackCleared = null)
        {
            if (_selectionViewStack.Count == 0)
            {
                Debug.LogWarning("There is no current element!");
                ProcessDequeue();
                yield break;
            }
        
            IsCoroutineRunning = true;
        
            //clear current stack
            yield return CurrentElement.OnExit();
            _selectionViewStack.Pop();
            while (_selectionViewStack.Count > 0)
            {
                CurrentElement.OnExitImmediate();
                _selectionViewStack.Pop();
            }
            HasStack = false;
            onStackCleared?.Invoke();
        
            //dispose
            _requestQueue.Clear();
            yield return OnStackEmpty();
            IsCoroutineRunning = false;
        }
    
        private IEnumerator OnInitialize()
        {
            OnBeforeInitialize();
            yield return new WaitForSeconds(InitializationTime);
            HasStack = true;
            OnAfterInitialize();
        }
    
        #endregion

        #region Abstraction
    
        /// <summary>
        /// Will be called, as soon as a first element got added to the stack
        /// </summary>
        protected abstract void OnBeforeInitialize();
    
        /// <summary>
        /// Will be called after the initialization time. TODO: check, what happens with multiple registrations at the same time (cause coroutines are weird)
        /// </summary>
        protected abstract void OnAfterInitialize();
    
        /// <summary>
        /// Will be called after the stack became empty.
        /// </summary>
        protected abstract IEnumerator OnStackEmpty();
    
        #endregion
    }
}
