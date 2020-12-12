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
        public float FallVelocity { get; set; }
        private CharacterController _characterController;
        private Vector3 _movePlayer;
        public Transform cameraTransform;


        void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            if (IsDangeours())
            {
                enabled = false;
            }

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

        private void SetMovement()
        {
            var motion = _movePlayer * (movementSpeed * Time.deltaTime);
            if (VirtualInputManager.Instance.Run)
                motion *= runVelocity;
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
            return Physics.Raycast(transform.position,
                Vector3.down * 0.01f,
                0.01f);
        }

        public bool IsDangeours()
        {
            return Physics.Raycast(transform.position,
                Vector3.down * 0.01f,
                0.01f, LayerMask.GetMask("Dangeours"));
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
            Gizmos.color = Color.red;

            Gizmos.DrawRay(transform.position, Vector3.down * 0.01f);
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
    }
}