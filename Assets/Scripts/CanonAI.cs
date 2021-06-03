using Photon.Pun;
using UnityEngine;

public class CanonAI : MonoBehaviour
{

    private bool horizontal;
    private bool followPlayerTop;
    private float fireRange = 50;
    private Vector3 playerToFollowPos;
    private GameObject playerToFollow;
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
            playerToFollow = GameObject.Find("PlayerTop(Clone)");
            playerToFollowPos = playerToFollow.transform.position;
        }
        else
        {
            playerToFollow = GameObject.Find("PlayerBot(Clone)");
            playerToFollowPos = playerToFollow.transform.position;
        }

        if (Vector2.Distance(transform.position, playerToFollowPos) <= fireRange && playerToFollow.GetComponent<PlayerMovement>().isDead == false)
        {
            // Aim at the player (change rotation)
            var dir = playerToFollowPos - transform.position;
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
        // GameObject ball = Instantiate(fireBall, transform);
        PhotonNetwork.Instantiate(fireBall.name, new Vector2(transform.position.x - 0.1f, transform.position.y), Quaternion.identity, 1);
    }

    public Vector3 GetPlayerToFollowPos()
    {
        return playerToFollowPos;
        // return new Vector3(playerToFollowPos.x * 10, playerToFollowPos.y * 10);
    }
}
