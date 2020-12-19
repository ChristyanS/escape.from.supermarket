using System.Collections;
using Behaviours.Actions;
using Behaviours.Managers;
using UnityEngine;

namespace Behaviours.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        public float movementSpeed = 1.7f;
        public float gravity = 9.8f;
        public float jumpForce = 3;
        public float runVelocity = 1.5f;
        public float timeToDie = 2f;
        public float reduceVelocity = 2f;
        public float FallVelocity { get; set; }
        private CharacterController _characterController;
        private Vector3 _movePlayer;
        public Transform cameraTransform;
        private float _timeToDieAux;
        public GameObject checkGround;
        public bool IsDied { get; set; }
        public bool IsPushing { get; set; }
        private bool _isInjured;
        public bool IsWinner { get; set; }

        void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private IEnumerator Timer()
        {
            CameraShake.Instance.ShakeCamera(0.6f);
            _isInjured = true;
            yield return new WaitForSeconds(0.1f);
            CameraShake.Instance.ShakeCamera(0);
            yield return new WaitForSeconds(1);
            _isInjured = false;
        }

        void Update()
        {
            IsPushing = false;

            if (IsDangerous())
            {
                if (!_isInjured)
                {
                    StartCoroutine(Timer());
                }

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
                if (VirtualInputManager.Instance.Jump && !IsDangerous())
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
                BlackAndWhiteEffect.Instance.Enable();
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

        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider != null && hit.collider.CompareTag("Pushable"))
            {
                IsPushing = true;
                hit.collider.attachedRigidbody.isKinematic = !VirtualInputManager.Instance.Push;
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

        private void TimeTiDie()
        {
            _timeToDieAux += 1 * Time.deltaTime;
            if (_timeToDieAux > timeToDie)
            {
                BlackAndWhiteEffect.Instance.Enable();
                IsDied = true;
                enabled = false;
            }
        }
    }
}