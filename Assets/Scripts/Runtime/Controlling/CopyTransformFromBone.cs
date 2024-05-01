using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VENTUS.Controlling
{
    public class CopyTransformFromBone : MonoBehaviour
    {
        [SerializeField]
        private Transform _bone;

        [Header("enabled Transforms")]
        [SerializeField] bool _position;
        [SerializeField] bool _rotation;
        [SerializeField] bool _scale;
        [SerializeField] bool _keepOffset;

        private void Update() {
            if (_position) {
                this.transform.position = _bone.position;
            }
            if (_rotation) {
                this.transform.rotation = _bone.rotation;
            }
            if (_scale) {
                this.transform.localScale = _bone.localScale;
            }
        }
    }
}
