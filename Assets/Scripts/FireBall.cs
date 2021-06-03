
using System;
using UnityEngine;


public class FireBall : MonoBehaviour
{


    public Transform playerPos;
    private float speed = 10;
    private BoxCollider2D BoxCollider2D;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerPos.position, speed * Time.deltaTime);
    }
    

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Explode
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("YESS");
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Platform"))
        {
            Destroy(gameObject);
        }
    }
}
