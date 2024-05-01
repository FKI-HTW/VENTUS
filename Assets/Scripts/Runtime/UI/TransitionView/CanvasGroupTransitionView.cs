using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace VENTUS.UI.TransitionView
{
    public class CanvasGroupTransitionView : BaseTransitionView
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] protected float enableTime;
        [SerializeField] protected float disableTime;
        
        protected override void OnSetup()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
        
        protected override IEnumerator OnSetActiveTransition()
        {
            _canvasGroup.DOFade(1f, enableTime).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            yield return new WaitForSeconds(enableTime);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        protected override IEnumerator OnSetInactiveTransition()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.DOFade(0f, disableTime).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            yield return new WaitForSeconds(disableTime);
        }
    }
}
