using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPunCallbacks
{

    private float x;
    public float speed;
    private Rigidbody2D rigidbody;
    public float jumpForce;
    private bool jump;

    private Vector3 movement;
    // [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    // public static GameObject LocalPlayerInstance;

    // private void Awake()
    // {
    //     if (photonView.IsMine)
    //     {
    //         PlayerMovement.LocalPlayerInstance = this.gameObject;
    //     }
    //     DontDestroyOnLoad(this.gameObject);
    // }
    
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     transform.position = new Vector2(0f, 5f);
        // }
        // else
        // {
        //     transform.position = new Vector2(0f, -5f);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected)
        {
            return;
        }
        x = Input.GetAxis("Horizontal"); // left = -1, right = 1
        jump = Input.GetButtonDown("Jump");
        // if (x > 0)
        // {
        //     transform.Translate(transform.forward * Time.deltaTime * speed, Space.World);
        // }
        // else if (x < 0)
        // {
        //     transform.Translate(-transform.forward * Time.deltaTime * speed, Space.World);
        // }
        // transform.position += (Vector3) new Vector2(x*speed*Time.deltaTime, 0);
        // transform.position += new Vector3(x*speed*Time.deltaTime, 0);
        //Move Front/Back
        // if (Input.GetKey(KeyCode.Z))
        // {
        //     transform.Translate(transform.forward * Time.deltaTime * speed, Space.World);
        // }
        // else if (Input.GetKey(KeyCode.S))
        // {
        //     transform.Translate(-transform.forward * Time.deltaTime * speed, Space.World);
        // }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected)
        {
            return;
        }
        transform.position += new Vector3(x*speed*Time.deltaTime, 0);
        if (jump)
        {
            rigidbody.AddForce(transform.up*jumpForce, ForceMode2D.Impulse);
        }
    }
}
