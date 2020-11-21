using UnityEngine;
using UnityEngine.Animations;

namespace Behaviours
{
    public class PlayerController : MonoBehaviour
    {
        private CharacterController _characterController;
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

        void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        void FixedUpdate()
        {
            var horizontalAxis = Input.GetAxis("Horizontal");
            var verticalAxis = Input.GetAxis("Vertical");
            _direction = new Vector3(horizontalAxis, 0, verticalAxis);

            CameraDirection();
            _movePlayer = _direction.x * _cameraRight + _direction.z * _cameraForward;

            _characterController.transform.LookAt(_characterController.transform.position + _movePlayer);

            SetGravity();
            Jump();
            _characterController.Move(_movePlayer * (movementSpeed * Time.deltaTime));
        }

        void SetGravity()
        {
            if (_characterController.isGrounded)
                fallVelocity = -gravity * Time.deltaTime;
            else
                fallVelocity -= gravity * Time.deltaTime;
            _movePlayer.y = fallVelocity;
        }

        void Jump()
        {
            if (_characterController.isGrounded && Input.GetButtonDown("Jump"))
            {
                fallVelocity = jumpForce;
                _movePlayer.y = fallVelocity;
            }
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
    }
}