using System;
using Enuns;
using UnityEngine;

namespace Behaviours.Managers
{
    public class VirtualInputManager : Singleton<VirtualInputManager>
    {
        public bool enableJump;
        public bool enableRun;
        public bool enablePush;
        public bool enableWalk;
        public Vector3 Direction { get; private set; }
        public float HorizontalAxis { get; private set; }
        public float VerticalAxis { get; private set; }
        public bool Jump { get; private set; }
        public bool Run { get; private set; }
        public bool Push { get; private set; }

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
            Direction = enableWalk ? new Vector3(HorizontalAxis, 0, VerticalAxis) : Vector3.zero;
        }

        private void CheckJump()
        {
            Jump = Input.GetButtonDown("Jump") && enableJump;
        }

        private void CheckRun()
        {
            Run = Input.GetKey(KeyCode.LeftShift) && enableRun;
        }

        private void CheckPush()
        {
            Push = Input.GetKey(KeyCode.Mouse0) && enablePush;
        }

        public void EnableAllControls(bool enable)
        {
            enableJump = enable;
            enableRun = enable;
            enablePush = enable;
            enableWalk = enable;
        }
        public void EnableControl(Mechanics mechanics, bool enable)
        {
            switch (mechanics)
            {
                case Mechanics.Walk:
                    enableWalk = enable;
                    break;
                case Mechanics.Jump:
                    enableJump = enable;
                    break;
                case Mechanics.Push:
                    enablePush = enable;
                    break;
                case Mechanics.Run:
                    enableRun = enable;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mechanics), mechanics, "Mechanic not implemented");
            }
        }
        
    }
}