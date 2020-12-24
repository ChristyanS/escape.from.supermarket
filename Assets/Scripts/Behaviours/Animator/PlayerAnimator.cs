using Behaviours.Controllers;
using Behaviours.Managers;
using UnityEngine;

namespace Behaviours.Animator
{
    public class PlayerAnimator : MonoBehaviour
    {
        private UnityEngine.Animator _animator;
        private PlayerController _playerController;
        private static readonly int Forward = UnityEngine.Animator.StringToHash("forward");
        private static readonly int Run = UnityEngine.Animator.StringToHash("run");
        private static readonly int Die = UnityEngine.Animator.StringToHash("die");
        private static readonly int Push = UnityEngine.Animator.StringToHash("push");
        private static readonly int IsGround = UnityEngine.Animator.StringToHash("isGround");
        private static readonly int Fall = UnityEngine.Animator.StringToHash("fall");
        private static readonly int IsWinner = UnityEngine.Animator.StringToHash("IsWinner");

        void Start()
        {
            _animator = GetComponent<UnityEngine.Animator>();
            _playerController = GetComponent<PlayerController>();
        }

        void Update()
        {
            CheckForwardAnimation();
            CheckRunAnimation();
            CheckDeathAnimation();
            CheckPushAnimation();
            CheckGround();
            CheckFall();
            CheckRagdoll();
            CheckWin();
        }

        private void CheckRagdoll()
        {
            if (_playerController.IsDeadByFall() ||_playerController.IsHit)
            {
                _animator.enabled = false;
            }
        }

        private void CheckForwardAnimation()
        {
            if (_playerController.IsWalking)
                _animator.SetFloat(Forward,
                    Mathf.Abs(VirtualInputManager.Instance.VerticalAxis) >
                    Mathf.Abs(VirtualInputManager.Instance.HorizontalAxis)
                        ? Mathf.Abs(VirtualInputManager.Instance.VerticalAxis)
                        : Mathf.Abs(VirtualInputManager.Instance.HorizontalAxis));
        }

        private void CheckRunAnimation()
        {
            _animator.SetBool(Run, VirtualInputManager.Instance.Run);
        }

        private void CheckDeathAnimation()
        {
            _animator.SetBool(Die, _playerController.IsDied);
        }

        private void CheckPushAnimation()
        {
            _animator.SetBool(Push, _playerController.IsPushing);
        }

        private void CheckGround()
        {
            _animator.SetBool(IsGround, _playerController.IsGround());
        }

        private void CheckFall()
        {
            _animator.SetFloat(Fall, _playerController.FallVelocity);
        }

        private void CheckWin()
        {
            _animator.SetBool(IsWinner, _playerController.IsWinner);
        }
    }
}