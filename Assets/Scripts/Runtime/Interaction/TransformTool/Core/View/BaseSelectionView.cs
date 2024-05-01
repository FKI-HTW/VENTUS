using System.Collections;
using UnityEngine;
using VENTUS.Interaction.TransformTool.UI.Transformer;
using VENTUS.UI;
using VENTUS.UI.TransitionView;

namespace VENTUS.Interaction.TransformTool.Core.View
{
    public class BaseSelectionView : BaseSelectionElement, ITransformToolView
    {
        #region Fields
     
        [Header("Position")]
        [SerializeField] private BasePositioner _positioner;
        [SerializeField] private BaseRotator _rotator;
    
        public MonoBehaviour MonoBehaviour => this;
        
        #endregion

        #region Inheritance

        protected override void InternalInitialize()
        {
            base.InternalInitialize();

            SetPosition();
            SetRotation();
        }

        public virtual IEnumerator OnEnter()
        {
            yield return gameObject.EnableCoroutine();
        }

        public virtual IEnumerator OnFocus()
        {
            yield return gameObject.EnableCoroutine();
        }

        public virtual IEnumerator OnRelease()
        {
            yield return gameObject.DisableCoroutine(DeactivationType.Disable);
        }

        public virtual IEnumerator OnExit()
        {
            yield return gameObject.DisableCoroutine(DeactivationType.Destroy);
        }

        public virtual void OnExitImmediate()
        {
            Destroy(gameObject);
        }
        
        #endregion
        
        #region Protected Methods
        
        protected void SetRotation()
        {
            if (_rotator != null)
            {
                _rotator.UpdateRotation(transform, SelectableGo);
            }
        }

        protected void SetPosition()
        {
            if (_positioner != null)
            {
                transform.position = _positioner.GetUpdatedPosition(SelectableGo);
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }
        }
        
        #endregion
        
        #region Public Methods
        
        public virtual void Close()
        {
            SelectionViewStackMachine.ClearStack();
        }
        
        public virtual void NavigateBack()
        {
            SelectionViewStackMachine.PopStackElement();
        }
        
        #endregion
    }
}
