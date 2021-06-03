using System;
using Photon.Pun;
using UnityEngine;


public class FireBall : MonoBehaviour
{


    private Vector3 playerPos;
    private float speed = 10;
    private BoxCollider2D BoxCollider2D;
    private Rigidbody2D rigidbody2D;
    public GameObject explosionPrefab;
    public int dmg = 30;
    
    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.Find("Canon").GetComponent<CanonAI>().GetPlayerToFollowPos();
        rigidbody2D = GetComponent<Rigidbody2D>();
        // GetComponent<Rigidbody2D>().velocity = playerPos * speed;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Vector2.Distance(transform.position, playerPos) <= 0)
        // {
        //     playerPos *= 2;
        // }
        transform.position = Vector2.MoveTowards(transform.position, playerPos, speed * Time.deltaTime);
        transform.Rotate(0,0,20*Time.deltaTime);
    }
    

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Explode
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerMovement>().TakeDamage(dmg);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Platform"))
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.Instantiate(explosionPrefab.name, transform.position, Quaternion.identity, 1);
        
    }
}
