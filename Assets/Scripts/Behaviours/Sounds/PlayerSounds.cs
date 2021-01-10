using UnityEngine;
using UnityEngine.Serialization;

namespace Behaviours.Sounds
{
    public class PlayerSounds : MonoBehaviour
    {
        [SerializeField] private AudioClip[] walkClip;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip dyingClip;
        [SerializeField] private AudioClip fallingClip;
        [SerializeField] private AudioClip injuredClip;
        [SerializeField] private AudioClip splashClip;
        [SerializeField] private AudioClip deathEffectClip;
        private AudioSource _audioSource;
        private int _auxWalk;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Step()
        {
            _audioSource.PlayOneShot(walkClip[_auxWalk]);
            SetAuxWalk();
        }

        void SetAuxWalk()
        {
            if (_auxWalk == walkClip.Length-1)
            {
                _auxWalk = 0;
            }
            else
            {
                _auxWalk++;
            }
        }
        private void Jump()
        {
            _audioSource.PlayOneShot(jumpClip);
        }

        private void Dying()
        {
            _audioSource.PlayOneShot(dyingClip);
        }

        private void Falling()
        {
            _audioSource.PlayOneShot(fallingClip);
        }

        public void Injured()
        {
            _audioSource.PlayOneShot(injuredClip);
        }
        public void Splash()
        {
            _audioSource.PlayOneShot(splashClip);
        }
        
        public void DeathEffectClip()
        {
            _audioSource.PlayOneShot(deathEffectClip);
        }
    }
}