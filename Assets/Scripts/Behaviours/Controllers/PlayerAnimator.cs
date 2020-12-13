using Behaviours.Managers;
using UnityEngine;

namespace Behaviours.Controllers
{
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator _animator;
        private PlayerController _playerController;
        private static readonly int Forward = Animator.StringToHash("forward");
        private static readonly int Run = Animator.StringToHash("run");
        private static readonly int Die = Animator.StringToHash("die");
        private static readonly int Push = Animator.StringToHash("push");
        private static readonly int IsGround = Animator.StringToHash("isGround");
        private static readonly int Fall = Animator.StringToHash("fall");

        void Start()
        {
            _animator = GetComponent<Animator>();
            _playerController = GetComponent<PlayerController>();
        }

        void Update()
        {
            CheckFowardAnimation();
            CheckRunAnimation();
            CheckDeathAnimation();
            CheckPushAnimation();
            CheckGround();
            CheckFall();
        }

        private void CheckFowardAnimation()
        {
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
            _animator.SetBool(Die, _playerController.isDied);
        }

        private void CheckPushAnimation()
        {
            _animator.SetBool(Push, VirtualInputManager.Instance.Push);
        }
        
        private void CheckGround()
        {
            _animator.SetBool(IsGround, _playerController.IsGround());
        }
        
        private void CheckFall()
        {
            _animator.SetFloat(Fall, _playerController.FallVelocity);
        }

    }
}