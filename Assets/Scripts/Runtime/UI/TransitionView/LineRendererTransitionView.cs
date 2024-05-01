using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace VENTUS.UI.TransitionView
{
    public class LineRendererTransitionView : BaseTransitionView
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _enableTime;
        [SerializeField] private float _disableTime;
        
        private Color _startColor;
        private Color _endColor;

        protected override void OnSetup()
        {
            _startColor = _lineRenderer.startColor;
            _endColor = _lineRenderer.endColor;
        }

        protected override IEnumerator OnSetActiveTransition()
        {
            Color currentColor = new Color(0, 0, 0, 0);
            _lineRenderer.DOColor(new Color2(currentColor, currentColor), new Color2(_startColor, _endColor), _enableTime)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            
            yield return new WaitForSeconds(_enableTime);
        }

        protected override IEnumerator OnSetInactiveTransition()
        {
            Color newColor = new Color(0, 0, 0, 0);
            _lineRenderer.DOColor(new Color2(_startColor, _endColor), new Color2(newColor, newColor), _disableTime)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            
            yield return new WaitForSeconds(_disableTime);
        }
    }
}
