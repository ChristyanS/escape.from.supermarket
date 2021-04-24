using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomTriggerAndCollisionEvents : MonoBehaviour
{
    public UnityEvent MyGamesTriggerEnter;
    public UnityEvent MyGamesTriggerStay;
    public UnityEvent MyGamesTriggerExit;
    public UnityEvent MyGamesCollisionEnter;
    public UnityEvent MyGamesCollisionStay;
    public UnityEvent MyGamesCollisionExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) MyGamesTriggerEnter.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) MyGamesTriggerStay.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) MyGamesTriggerExit.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) MyGamesCollisionEnter.Invoke();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) MyGamesCollisionStay.Invoke();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) MyGamesCollisionExit.Invoke();
    }
}
