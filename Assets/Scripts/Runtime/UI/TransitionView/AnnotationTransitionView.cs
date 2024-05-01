using System.Collections;
using DG.Tweening;
using UnityEngine;
using VENTUS.Interaction.TransformTool.UI.Transformer;

namespace VENTUS.UI.TransitionView
{
    public class AnnotationViewTransitionView : CanvasGroupTransitionView
    {
        [SerializeField] private BasePositioner _positioner;
        
        private Vector3 _entryScale;
        private Vector3 _entryPosition;
        
        protected override void OnSetup()
        {
            base.OnSetup();

            var thisTransform = transform;
            _entryScale = thisTransform.localScale;
            _entryPosition = thisTransform.position;
        }

        protected override IEnumerator OnSetActiveTransition()
        {
            var thisTransform = transform;
            
            thisTransform.localPosition = _entryPosition;
            Vector3 targetPosition = _positioner.GetUpdatedPosition(gameObject);
            transform.DOMove(targetPosition, enableTime).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            
            thisTransform.localScale = Vector3.zero;
            transform.DOScale(_entryScale, enableTime).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            
            yield return base.OnSetActiveTransition();
        }

        protected override IEnumerator OnSetInactiveTransition()
        {
            transform.DOMove(_entryPosition, disableTime).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            transform.DOScale(Vector3.zero, disableTime).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            
            yield return base.OnSetInactiveTransition();
        }
    }
}
