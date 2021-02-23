using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDeWish : MonoBehaviour
{
    
    public Transform playerTop;
    public Transform playerBot;
    private bool follow = false;
    private float beginning;
    // private float moveSpeed = .02f;

    // Start is called before the first frame update
    void Start()
    {
        beginning = transform.position.x;
    }

    private void Update()
    {
        if (playerTop.transform.position.x > transform.position.x)
        {
            follow = true;
        }

        if (playerTop.transform.position.x < beginning)
            follow = false;
    }

    void FixedUpdate()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 temp = transform.position;
        if (follow)
        {
            temp.x = playerTop.position.x;

            transform.position = temp;   
        }

        // temp.x += moveSpeed;
        // transform.position = temp;
    }
    
}
