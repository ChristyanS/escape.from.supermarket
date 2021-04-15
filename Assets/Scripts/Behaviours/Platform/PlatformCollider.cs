using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCollider : MonoBehaviour
{

    public GameObject Player;


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with something");
        if (collision.gameObject.CompareTag("Player"))
        {

                    Player.transform.parent = transform;
            Debug.Log("Collided with player");
        }
        
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            Player.transform.parent = null;

        }
    }





    //private void OnTriggerEnter(Collider other)
    //{
    //   if(other.gameObject == Player)
    //    {

    //        Player.transform.parent = transform;

    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject == Player)
    //    {
    //        Player.transform.parent = null;

    //    }
    //}


}
