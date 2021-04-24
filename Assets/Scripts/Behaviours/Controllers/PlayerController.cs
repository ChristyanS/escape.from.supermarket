using System.Collections;
using Behaviours.Actions;
using Behaviours.Managers;
using Behaviours.Sounds;
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
        [SerializeField] [Range(0, 10)] private float slopeForce = 4;
        [SerializeField] [Range(0, 20)] private float bouncingForce = 3;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private GameObject checkGround;
        [SerializeField] private GameObject checkForward;
        [SerializeField] private GameObject startCapsule;
        [SerializeField] private GameObject endCapsule;
        private CharacterController _characterController;
        private PlayerSounds _playerSounds;
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
            _playerSounds = GetComponent<PlayerSounds>();
            transform.position = VirtualInputManager.Instance.spawnPosition;
            Physics.SyncTransforms();
        }

        private bool OnSlope()
        {
            if (Physics.Raycast(transform.position,
                Vector3.down * 0.01f, out var hit,
                0.1f))
            {
                if (hit.normal != Vector3.up)
                {
                    var a = Vector3.Angle(hit.normal, _characterController.transform.forward);
                    return true;
                }
                
            }
            return false;
        }

        private IEnumerator DamageEffect()
        {
            CameraShake.Instance.ShakeCamera(0.6f);
            _isInjured = true;
            _playerSounds.Injured();
            yield return new WaitForSeconds(0.1f);
            CameraShake.Instance.ShakeCamera(0);
            yield return new WaitForSeconds(1);
            _isInjured = false;
        }

        private void SetDeathByCollision()
        {
            if (WasHit())
            {
                IsHit = true;
                SetDeath();
            }
        }

        private bool WasHit()
        {
            return Physics.CheckCapsule(startCapsule.transform.position, endCapsule.transform.position,
                _characterController.radius,
                LayerMask.GetMask("Dangerous"));
        }
        
        private bool WasHitBouncing()
        {
            return Physics.CheckCapsule(startCapsule.transform.position, endCapsule.transform.position,
                _characterController.radius,
                LayerMask.GetMask("Bouncing"));
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
                SetBouncing();
                if (OnSlope())
                {
                    SetJumping();
                }
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

        public void SetBouncing()
        {
            if (WasHitBouncing())
            {
                FallVelocity = bouncingForce;
                _movePlayer.y = FallVelocity;
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
            {
                SetRunVelocity();
            }

            _characterController.Move(_movePlayer);
            if (OnSlope() && ! HasObjectForward(out var hit))
            {
                _characterController.Move(Vector3.down * _characterController.height / 2 * slopeForce * Time.deltaTime);
            }
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
            return Physics.CheckSphere(checkGround.transform.position, _characterController.radius,
                LayerMask.GetMask("Default"));
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
            Gizmos.DrawWireSphere(position, 0.04f);
            Gizmos.DrawRay(position, Vector3.down * 0.35f);
            Gizmos.DrawRay(checkForward.transform.position, checkForward.transform.forward * 0.1f);

        }

        private void SetPushable()
        {
            var detected = HasObjectForward(out var hit);
            if (detected && hit.transform.CompareTag("Pushable"))
            {
                if (VirtualInputManager.Instance.Push)
                {
                    var c = hit.collider.GetComponent<CharacterController>();
                    var move = new Vector3(_movePlayer.x / reduceVelocityPushableObject, 0,
                        _movePlayer.z / reduceVelocityPushableObject);
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

        private bool HasObjectForward(out RaycastHit hit)
        {
            if (Physics.Raycast(checkForward.transform.position,
                checkForward.transform.forward * 0.1f, out hit,
                0.35f))
            {
                if (!hit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }

            return false;
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
            _playerSounds.Splash();
            _playerSounds.DeathEffectClip();
        }
    }
}