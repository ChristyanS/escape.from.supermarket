using UnityEngine;

namespace Behaviours.Managers
{
    public class SoundsManager : Singleton<SoundsManager>
    {
        private AudioSource[] _audioSources;
        [SerializeField] [Range(0, 1)] private float volume = 1;

        private void Start()
        {
            _audioSources = FindObjectsOfType<AudioSource>();
            foreach (var audioSource in _audioSources)
            {
                audioSource.volume = volume;
            }
        }
    }
}