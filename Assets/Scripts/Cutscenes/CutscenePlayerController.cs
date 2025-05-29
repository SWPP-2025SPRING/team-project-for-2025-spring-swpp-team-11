using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CutscenePlayerController : MonoBehaviour
{
    private Animator playerAnim;

    //movement
    public float verticalInput;
    public float speed = 13.0f;
    public float destination = 14f;
    private bool shouldMove = true;

    private void Start()
    {
        playerAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        if ((verticalInput != 0) && shouldMove)
        {
            transform.Translate(Vector3.forward * verticalInput * Time.deltaTime * speed);
            playerAnim.SetFloat("Speed_f", 6.0f);
        }

        if (transform.position.z > 14)
        {
            shouldMove = false;
            //transform.position = new Vector3(transform.position.x, transform.position.y, destination);
            playerAnim.SetTrigger("Idle_trig");
            playerAnim.SetInteger("Idle_int", 1);
        }
    }

}
