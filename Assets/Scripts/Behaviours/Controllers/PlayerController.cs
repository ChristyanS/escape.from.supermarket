using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Behaviours.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        private CharacterController _characterController;
        private Animator _animator;
        private Vector3 _direction;
        private float _acceleration;
        private Vector3 _cameraRight;
        private Vector3 _cameraForward;
        private Vector3 _movePlayer;
        public Transform cameraTransform;
        public float movementSpeed = 2.0f;
        public float gravity = 9.8f;
        public float fallVelocity;
        public float jumpForce;
        public float runVelocity = 2;
        public LayerMask dangeours;
        private bool _isDead;
        public float pushPower = 1;

        void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
        }

        void Update()
        {
            var horizontalAxis = Input.GetAxis("Horizontal");
            var verticalAxis = Input.GetAxis("Vertical");
            _direction = new Vector3(horizontalAxis, 0, verticalAxis);

            _animator.SetFloat("forward",
                Mathf.Abs(verticalAxis) > Mathf.Abs(horizontalAxis)
                    ? Mathf.Abs(verticalAxis)
                    : Mathf.Abs(horizontalAxis));

            if (IsDangeours() && !_isDead)
            {
                _animator.SetBool("die", true);
                enabled = false;
            }

            if (IsGround())
            {
                _animator.SetBool("isGround", IsGround());

                fallVelocity = 0;
                if (Input.GetButtonDown("Jump"))
                {
                    fallVelocity = jumpForce;
                }
            }
            else
            {
                _animator.SetBool("isGround", IsGround());
                fallVelocity -= gravity * Time.deltaTime;
            }

            _animator.SetFloat("fall", fallVelocity);

            CameraDirection();
            _movePlayer = _direction.x * _cameraRight + _direction.z * _cameraForward;
            _characterController.transform.LookAt(_characterController.transform.position + _movePlayer);
            _direction = _movePlayer;
            _direction.y = fallVelocity;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _animator.SetBool("run", true);
                _characterController.Move(_direction * (movementSpeed * runVelocity * Time.deltaTime));
            }
            else
            {
                _animator.SetBool("run", false);
                _characterController.Move(_direction * (movementSpeed * Time.deltaTime));
            }

            if (!Input.GetKey(KeyCode.Mouse0))
            {
                _animator.SetBool("push", false);
            }
        }

        private bool IsGround()
        {
            return Physics.Raycast(transform.position,
                Vector3.down * 0.01f,
                0.01f);
        }

        private bool IsDangeours()
        {
            return Physics.Raycast(transform.position,
                Vector3.down * 0.01f,
                0.01f, dangeours);
        }

        void CameraDirection()
        {
            _cameraForward = cameraTransform.forward;
            _cameraRight = cameraTransform.right;

            _cameraForward.y = 0;
            _cameraRight.y = 0;

            _cameraForward = _cameraForward.normalized;
            _cameraRight = _cameraRight.normalized;
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
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    var pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
                    body.velocity += pushDirection * pushPower;
                    _animator.SetBool("push", true);
                }
                else
                {
                    _animator.SetBool("push", false);
                }
            }
        }
    }
}