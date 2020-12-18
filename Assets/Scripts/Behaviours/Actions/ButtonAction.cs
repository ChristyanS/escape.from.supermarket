using UnityEngine;

namespace Behaviours.Actions
{
    public class ButtonAction : MonoBehaviour
    {
        public GameObject targetObject;
        private bool _trigged;
        private static readonly int Trigger = UnityEngine.Animator.StringToHash("trigger");

        private void OnCollisionEnter(Collision other)
        {
            if (!_trigged)
            {
                GetComponentInParent<UnityEngine.Animator>().SetBool(Trigger,true);
                targetObject.GetComponent<UnityEngine.Animator>().SetBool(Trigger, true);
                _trigged = true;
            }
        }
    }
}