using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Timeline;

public class CanonAI : MonoBehaviour
{

    private bool horizontal;
    private bool followPlayerTop;
    private float fireRange = 50;
    private Transform playerToFollowPos;
    private float angularSpeed = 1;
    private bool isRotating;
    private float fireCD = 2;
    private float fireCDStatus;
    public GameObject fireBall;
    
    // Start is called before the first frame update
    void Start()
    {
        fireCDStatus = fireCD;
        horizontal = Camera.main.GetComponent<CameraMovement>().horizontalLeft || Camera.main.GetComponent<CameraMovement>().horizontalRight;
        if (horizontal)
        {
            if (transform.position.y > GameObject.Find("Main Camera").transform.position.y)
            {
                followPlayerTop = true;
            }
            else
            {
                followPlayerTop = false;
            }
        }
        else
        {
            if (transform.position.x > GameObject.Find("Main Camera").transform.position.x)
            {
                followPlayerTop = true;
            }
            else
            {
                followPlayerTop = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayerTop)
        {
            playerToFollowPos = GameObject.Find("PlayerTop(Clone)").transform;
        }
        else
        {
            playerToFollowPos = GameObject.Find("PlayerBot(Clone)").transform;
        }

        if (Vector2.Distance(transform.position, playerToFollowPos.position) <= fireRange)
        {
            // Aim at the player (change rotation)
            var dir = playerToFollowPos.position - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Shoot him
            if (fireCDStatus <= 0)
            {
                // Shoot    
                Fire();
                fireCDStatus = fireCD;

            }
            else
            {
                fireCDStatus -= Time.deltaTime;
            }
        }
    }

    private void Fire()
    {
        GameObject ball = Instantiate(fireBall);
        ball.GetComponent<FireBall>().playerPos = playerToFollowPos;
        
    }
}
