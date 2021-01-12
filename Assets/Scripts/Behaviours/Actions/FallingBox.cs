using System;
using UnityEngine;

namespace Behaviours.Actions
{
    public class FallingBox : MonoBehaviour
    {
        
        private Rigidbody _rigidbody;

        private void Start()
        {
            _rigidbody = GetComponentInChildren<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _rigidbody.isKinematic = false;

            }
        }
    }
}
