using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Behaviours.Actions
{
    public class BlackAndWhiteEffect : MonoBehaviour
    {
        public static BlackAndWhiteEffect Instance { get; private set; }
        private Volume _volume;
        private ColorAdjustments _colorAdjustments;
        public GameObject panel;
        private void Start()
        {
            _volume = GetComponent<Volume>();
            _colorAdjustments = (ColorAdjustments) _volume.profile.components.First(c => c is ColorAdjustments);
            Instance = this;
        }

        public void Enable()
        {
            StartCoroutine(Time());
        }

        private IEnumerator Time()
        {
            while (_colorAdjustments.saturation.value >= -100)
            {
                yield return new WaitForSeconds(0.007f);
                _colorAdjustments.saturation.value += -1f;
                if (_colorAdjustments.saturation.value <= -70)
                {
                    panel.SetActive(true);

                }
            }
            
        }
    }
}