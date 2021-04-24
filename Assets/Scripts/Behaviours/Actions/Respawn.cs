using Behaviours.Managers;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        VirtualInputManager.Instance.spawnPosition = transform.position;
    }
}