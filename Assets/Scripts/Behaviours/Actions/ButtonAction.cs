using System.Collections;
using Behaviours.Managers;
using Cinemachine;
using UnityEngine;

namespace Behaviours.Actions
{
    public class ButtonAction : MonoBehaviour
    {
        public GameObject targetObject;
        private bool _trigged;
        private static readonly int Trigger = UnityEngine.Animator.StringToHash("trigger");
        public CinemachineFreeLook freeLook;
        public CinemachineVirtualCamera virtualCamera;


        private void OnCollisionEnter(Collision other)
        {
            if (!_trigged)
            {
                GetComponentInParent<UnityEngine.Animator>().SetBool(Trigger,true);

                freeLook.enabled = false;
                virtualCamera.enabled = true;
                targetObject.GetComponent<UnityEngine.Animator>().SetBool(Trigger, true);
                _trigged = true;
                freeLook.gameObject.GetComponent<UnityEngine.Animator>();
                VirtualInputManager.Instance.EnableAllControls(false);
                StartCoroutine(ChangeCamera());
            }
           
        }

        private IEnumerator ChangeCamera()
        {
           
            yield return new WaitForSeconds(2);
            freeLook.enabled = true;
            virtualCamera.enabled = false;
            VirtualInputManager.Instance.EnableAllControls(true);
        }
    }
}