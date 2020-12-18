﻿using Behaviours.Managers;
using UnityEngine;

namespace Behaviours.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        public float movementSpeed = 1.7f;
        public float gravity = 9.8f;
        public float jumpForce = 3;
        public float runVelocity = 1.5f;
        public float pushPower = 0.05f;
        public float timeToDie = 2f;
        public float reduceVelocity = 2f;
        public float FallVelocity { get; set; }
        private CharacterController _characterController;
        private Vector3 _movePlayer;
        public Transform cameraTransform;
        private float _timeToDieAux;
        public GameObject checkGround;
        public bool IsDied { get; set; }

        void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            if (IsDangerous())
            {
                TimeTiDie();
            }
            else
            {
                _timeToDieAux = 0;
                IsDied = false;
            }

            SetDeathPerFall();
            SetPlayerMoveDirection();
            SetPlayerLookAtDirection();

            if (IsGround())
            {
                FallVelocity = 0;
                if (VirtualInputManager.Instance.Jump)
                    SetJump();
            }
            else
                SetGravity();

            SetMovement();
        }

        private void SetDeathPerFall()
        {
            if (FallVelocity <= -6)
            {
                IsDied = true;
                _characterController.enabled = false;
                enabled = false;

            }
        }

        private void SetMovement()
        {
            var reduceVelocityAux = 1f;
            if (IsDangerous())
            {
                reduceVelocityAux = reduceVelocity;
            }

            var motion = _movePlayer * (movementSpeed / reduceVelocityAux * Time.deltaTime);
            if (VirtualInputManager.Instance.Run)
                motion = new Vector3(motion.x * runVelocity, motion.y, motion.z * runVelocity);
            _characterController.Move(motion);
        }

        private void SetJump()
        {
            FallVelocity = jumpForce;
            _movePlayer.y = FallVelocity;
        }

        private void SetGravity()
        {
            FallVelocity -= gravity * Time.deltaTime;
            _movePlayer.y = FallVelocity;
        }

        private void SetPlayerLookAtDirection()
        {
            _characterController.transform.LookAt(_characterController.transform.position + _movePlayer);
        }

        private void SetPlayerMoveDirection()
        {
            _movePlayer = VirtualInputManager.Instance.Direction.x * GetCameraRight() +
                          VirtualInputManager.Instance.Direction.z * GetCameraForward();
        }

        public bool IsGround()
        {
            return Physics.CheckSphere(checkGround.transform.position, 0.08f, LayerMask.GetMask("Default"));
        }

        private bool IsDangerous()
        {
            var detected =  Physics.Raycast(transform.position,
                Vector3.down * 0.01f, out var hit,
                1);
            return detected && hit.transform.CompareTag("Fire");
        }

        private Vector3 GetCameraForward()
        {
            var cameraForward = cameraTransform.forward;
            cameraForward.y = 0;
            return cameraForward.normalized;
        }

        private Vector3 GetCameraRight()
        {
            var cameraRight = cameraTransform.right;
            cameraRight.y = 0;
            return cameraRight.normalized;
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(checkGround.transform.position, 0.08f);
            Gizmos.DrawRay(checkGround.transform.position, Vector3.down* 0.35f);
        }

        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            var body = hit.collider.attachedRigidbody;
            if (body)
            {
                if (VirtualInputManager.Instance.Push)
                {
                    var pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
                    body.velocity += pushDirection * pushPower;
                }
            }
        }

        private void TimeTiDie()
        {
            _timeToDieAux += 1 * Time.deltaTime;
            if (_timeToDieAux > timeToDie)
            {
                IsDied = true;
                enabled = false;
            }
        }
    }
}