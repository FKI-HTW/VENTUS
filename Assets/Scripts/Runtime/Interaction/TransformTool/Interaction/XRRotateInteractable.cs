using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VENTUS.Interaction.TransformTool.Core;

namespace VENTUS.Interaction.TransformTool.Interaction
{
    public class XRRotateInteractable : XRTransformerInteractable
    {
        [SerializeField] private float _smoothSpeed = 15f;

        private readonly Vector3 _memberwiseScale = new(1, -1, -1);
        
        private Quaternion _entrySelectorRotation;
        private Quaternion _entrySelectableRotation;
        private Quaternion _previousRotation;

        protected override void OnSelectUpdate()
        {
            Quaternion selectorRotation = InteractorGroup.EnabledInteractionInstances.First().Origin.rotation;
            Quaternion selectorRotationDelta = selectorRotation * Quaternion.Inverse(_entrySelectorRotation);
            Vector3 deltaEuler = selectorRotationDelta.eulerAngles;
            Quaternion memberwiseScaledSelectorRotationDelta = Quaternion.Euler(new Vector3(deltaEuler.x * _memberwiseScale.x, deltaEuler.y * _memberwiseScale.y, deltaEuler.z * _memberwiseScale.z));
            
            Quaternion targetRotation = memberwiseScaledSelectorRotationDelta * _entrySelectableRotation;
            Quaternion newRotation = Quaternion.Lerp(_previousRotation, targetRotation, _smoothSpeed * Time.deltaTime);
            transform.rotation =  newRotation;
        }
        
        protected override void InternalOnFirstSelectEntered()
        {
            _entrySelectorRotation = InteractorGroup.EnabledInteractionInstances.First().Origin.rotation;
            _entrySelectableRotation = transform.rotation;
        }
    }
}
