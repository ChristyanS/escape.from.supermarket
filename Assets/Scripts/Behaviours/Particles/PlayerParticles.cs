using UnityEngine;

namespace Behaviours.Particles
{
    public class PlayerParticles : MonoBehaviour
    {
        public ParticleSystem dustParticleRunning;
        public ParticleSystem dustParticleWalk;

        public void DustParticleRunning()
        {
            dustParticleRunning.Play();
        }
        
        public void DustParticleWalk()
        {
            dustParticleWalk.Play();
        }
    }
}