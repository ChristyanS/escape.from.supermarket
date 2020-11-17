using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;
    public float moveSpeed;
    public float rotationSpeed;
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, Input.GetAxisRaw("Horizontal") * rotationSpeed * Time.deltaTime, 0));
        var direction =  transform.TransformDirection(new Vector3(0,0,Input.GetAxis("Vertical")));
        _characterController.SimpleMove(direction * moveSpeed);
    }
}
