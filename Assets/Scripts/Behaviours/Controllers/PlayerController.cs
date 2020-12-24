﻿using System.Collections;
using Behaviours.Actions;
using Behaviours.Managers;
using UnityEngine;

namespace Behaviours.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] [Range(0, 3)] private float movementSpeed = 1.7f;
        [SerializeField] [Range(1, 100)] private float gravity = 9.8f;
        [SerializeField] [Range(0, 20)] private float jumpForce = 3;
        [SerializeField] [Range(1, 4)] private float speedMultiplier = 1.5f;
        [SerializeField] [Range(0, 20)] private float timeToDie = 4f;
        [SerializeField] [Range(1, 5)] private float reduceVelocity = 3f;
        [SerializeField] [Range(1, 5)] private float reduceVelocityPushableObject = 3f;
        [SerializeField] [Range(-20, 0)] private float fallVelocityDeath = -6;
        public Transform cameraTransform;
        public GameObject checkGround;
        public GameObject checkForward;
        public GameObject startCapsule;
        public GameObject endCapsule;

        private CharacterController _characterController;
        private Vector3 _movePlayer;
        private float _timeToDieAux;
        private bool _isInjured;
        public float FallVelocity { get; private set; }
        public bool IsDied { get; private set; }
        public bool IsPushing { get; private set; }
        public bool IsWinner { get; private set; }
        public bool IsWalking { get; private set; }
        public bool IsHit { get; private set; }

        void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private IEnumerator DamageEffect()
        {
            CameraShake.Instance.ShakeCamera(0.6f);
            _isInjured = true;
            yield return new WaitForSeconds(0.1f);
            CameraShake.Instance.ShakeCamera(0);
            yield return new WaitForSeconds(1);
            _isInjured = false;
        }

        void SetDeathByCollision()
        {
            if (WasHit())
            {
                IsHit = true;
                SetDeath();
            }
        }

        public bool WasHit()
        {
            return Physics.CheckCapsule(startCapsule.transform.position, endCapsule.transform.position, 0.1f,
                LayerMask.GetMask("Dangerous"));
        }

        void Update()
        {
            SetPlayerMoveDirection();
            SetPlayerLookAtDirection();

            if (!IsDied)
            {
                SetPushable();
                SetDeathByPuddleTrap();
                SetDeathPerFall();
                SetDeathByCollision();
                if (IsGround())
                {
                    FallVelocity = 0;
                    SetJumping();
                }
                else
                    SetGravity();

                SetMovement();
            }
        }

        private void SetJumping()
        {
            if (CanJump())
            {
                FallVelocity = jumpForce;
                _movePlayer.y = FallVelocity;
            }
        }

        private void SetDeathByPuddleTrap()
        {
            if (IsDangerous())
            {
                if (!_isInjured)
                {
                    StartCoroutine(DamageEffect());
                }

                SetDeathCountdown();
            }
            else
            {
                _timeToDieAux = 0;
                IsDied = false;
            }
        }

        private bool CanJump()
        {
            return VirtualInputManager.Instance.Jump && !IsDangerous();
        }

        private void SetDeathPerFall()
        {
            if (IsDeadByFall())
            {
                SetDeath();
            }
        }

        public bool IsDeadByFall()
        {
            return FallVelocity <= fallVelocityDeath;
        }

        private void SetMovement()
        {
            var reduceVelocityAux = 1f;

            if (IsDangerous())
            {
                reduceVelocityAux = reduceVelocity;
            }

            var horizontalVelocity = movementSpeed / reduceVelocityAux;

            SetWalkVelocity(horizontalVelocity);

            if (VirtualInputManager.Instance.Run)
                SetRunVelocity();
            _characterController.Move(_movePlayer);
        }

        private void SetRunVelocity()
        {
            _movePlayer = new Vector3(_movePlayer.x * speedMultiplier, _movePlayer.y, _movePlayer.z * speedMultiplier);
        }

        private void SetWalkVelocity(float horizontalVelocity)
        {
            _movePlayer =
                new Vector3(_movePlayer.x * horizontalVelocity, _movePlayer.y * 1.7f,
                    _movePlayer.z * horizontalVelocity) *
                Time.deltaTime;
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
            if (_movePlayer != Vector3.zero)
            {
                IsWalking = true;
            }
        }

        public bool IsGround()
        {
            return Physics.CheckSphere(checkGround.transform.position, 0.08f, LayerMask.GetMask("Default"));
        }

        private bool IsDangerous()
        {
            var detected = Physics.Raycast(transform.position,
                Vector3.down * 0.01f, out var hit,
                0.01f);
            return detected && hit.transform.CompareTag("Dangerous");
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

            var position = checkGround.transform.position;
            Gizmos.DrawWireSphere(position, 0.08f);
            Gizmos.DrawRay(position, Vector3.down * 0.35f);
        }

        public void SetPushable()
        {
            var detected = Physics.Raycast(checkForward.transform.position,
                checkForward.transform.forward * 0.1f, out var hit,
                0.1f);
            if (detected && hit.transform.CompareTag("Pushable"))
            {
                if (VirtualInputManager.Instance.Push)
                {
                    var c = hit.collider.GetComponent<CharacterController>();
                    var move = new Vector3(_movePlayer.x / reduceVelocityPushableObject, 0, _movePlayer.z / reduceVelocityPushableObject);
                    c.Move(move * Time.deltaTime);
                    IsPushing = true;
                }
                else
                {
                    IsPushing = false;

                }
            
            }

            if (!detected)
            {
                IsPushing = false;
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Finish"))
            {
                IsWinner = true;
                enabled = false;
            }
        }

        private void SetDeathCountdown()
        {
            _timeToDieAux += 1 * Time.deltaTime;
            if (_timeToDieAux > timeToDie)
            {
                SetDeath();
            }
        }

        private void SetDeath()
        {
            BlackAndWhiteEffect.Instance.Enable();
            IsDied = true;
            VirtualInputManager.Instance.EnableAllControls(false);
        }
    }
}