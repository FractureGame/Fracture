using System;
using Photon.Pun;
using UnityEngine;


public class FireBall : MonoBehaviour
{


    private Vector2 beginning;
    private float speed = 10;
    private BoxCollider2D BoxCollider2D;
    private Rigidbody2D rigidbody2D;
    public GameObject explosionPrefab;
    private CircleCollider2D circleCollider2D;
    public int dmg = 30;
    
    // Start is called before the first frame update
    void Start()
    {
        beginning = transform.position;
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemies"))
        {
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerMovement>().TakeDamage(dmg);
            Destroy(gameObject);
        }
        else if (Vector2.Distance(transform.position, beginning) > 1 && other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("Danger"))
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.Instantiate(explosionPrefab.name, transform.position, Quaternion.identity, 1);
        
    }
}
