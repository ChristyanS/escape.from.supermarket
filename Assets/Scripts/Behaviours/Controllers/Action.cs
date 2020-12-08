using UnityEngine;

public class Action : MonoBehaviour
{
    public GameObject anyObject;
    public bool trigged;
    private void OnTriggerEnter(Collider other)
    {
        if (!trigged)
        {
            anyObject.transform.position += new Vector3(0, 0.5f, 0);
            trigged = true;
        }
    }
}