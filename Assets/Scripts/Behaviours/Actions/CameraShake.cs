using Cinemachine;
using UnityEngine;

namespace Behaviours.Actions
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }
        private CinemachineFreeLook _cinemachineFreeLook;

        void Start()
        {
            Instance = this;
            _cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
        }

        // Update is called once per frame
        public void ShakeCamera(float intensity)
        {
            for (var i = 0; i < CinemachineFreeLook.RigNames.Length; i++)
            {
                var cineBasicMultiChannelPerlin = _cinemachineFreeLook.GetRig(i)
                    .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
            }
            
        }
    }
}