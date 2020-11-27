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
        public LayerMask chaoLa;

        void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            var horizontalAxis = Input.GetAxis("Horizontal");
            var verticalAxis = Input.GetAxis("Vertical");
            _direction = new Vector3(horizontalAxis, 0, verticalAxis);
            if (Input.GetButtonDown("Jump") && EhChao())
            {
                fallVelocity = jumpForce;
            }

            fallVelocity -= gravity * Time.deltaTime ;
            
          
            CameraDirection();
            _movePlayer = _direction.x * _cameraRight + _direction.z * _cameraForward;
            _characterController.transform.LookAt(_characterController.transform.position + _movePlayer);
            _direction = _movePlayer;
            _direction.y = fallVelocity;
            _characterController.Move(_direction * (movementSpeed * Time.deltaTime));
        }

        public bool EhChao()
        {
            return Physics.Raycast(transform.position,
                Vector3.down * 0.7f,
                0.5f);
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

            Gizmos.DrawRay(transform.position, Vector3.down * 0.7f);
        }
    }
}