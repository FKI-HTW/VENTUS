using FishNet.Object;
using UnityEngine;

namespace VENTUS.Controlling
{
    public class AvatarAnimationSimple : NetworkBehaviour
    {
        [SerializeField] private Transform _headBone;
        [SerializeField] private Transform _bodyBone;
        [SerializeField] private Transform _hmdPosition;
        [SerializeField] private Transform _headDriver; // transform which drives the head

        private float _distanceHeadBody;

        private void Awake()
        {
            // dev node: some weird Pivot translation correction, occurred while exporting from blender
            _distanceHeadBody = _hmdPosition.position.z - _headDriver.position.z;
        }

        private void Update()
        {
            Vector3 avatarRotation = new(0, _headDriver.localEulerAngles.y, 0);
            _bodyBone.localRotation = Quaternion.Euler(avatarRotation);
            Vector3 avatarHeadRotation = new(_headDriver.localEulerAngles.x, transform.eulerAngles.y, _headDriver.localEulerAngles.z);
            _headBone.localRotation = Quaternion.Euler(avatarHeadRotation);
            _bodyBone.position = new(_headDriver.position.x, _headDriver.position.y - _distanceHeadBody, _headDriver.position.z);
        }
    }
}
