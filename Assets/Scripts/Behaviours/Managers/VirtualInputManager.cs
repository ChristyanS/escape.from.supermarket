using UnityEngine;

namespace Behaviours.Managers
{
    public class VirtualInputManager : Singleton<VirtualInputManager>
    {
        public Vector3 Direction { get; set; }
        public float HorizontalAxis { get; set; }
        public float VerticalAxis { get; set; }
        public bool Jump { get; set; }
        public bool Run { get; set; }
        public bool Push { get; set; }

        private void Update()
        {
            CheckDirection();
            CheckJump();
            CheckRun();
            CheckPush();
        }

        private void CheckDirection()
        {
            HorizontalAxis = Input.GetAxis("Horizontal");
            VerticalAxis = Input.GetAxis("Vertical");
            Direction = new Vector3(HorizontalAxis, 0, VerticalAxis);
        }

        private void CheckJump()
        {
            Jump = Input.GetButtonDown("Jump");
        }

        private void CheckRun()
        {
            Run = Input.GetKey(KeyCode.LeftShift);
        }

        private void CheckPush()
        {
            Push = Input.GetKey(KeyCode.Mouse0);
        }
        
    }
}