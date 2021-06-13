using Photon.Pun;
using UnityEngine;

public class StrongSpot : MonoBehaviourPunCallbacks
{

    private BoxCollider2D boxCollider2d;
    
    // Start is called before the first frame update
    void Start()
    {
        boxCollider2d = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("WeakSpot"))
        {
            Debug.LogFormat("PIEDS {0}", collision.transform.parent.parent.name);
            
            photonView.RPC("KillEnemy", RpcTarget.All, collision.transform.parent.parent.name);
            transform.parent.gameObject.GetComponent<PlayerMovement>().animator.SetBool("isJumping", true);
            transform.parent.gameObject.GetComponent<PlayerMovement>().animator.SetTrigger("jump");
            
            transform.parent.gameObject.GetComponent<PlayerMovement>().Jump();
            
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
            Physics2D.IgnoreCollision(other.collider, boxCollider2d);
        }

        if (other.gameObject.CompareTag("Pieds"))
        {
            Physics2D.IgnoreCollision(other.collider, boxCollider2d);
        }
        
        if (other.gameObject.CompareTag("Danger"))
        {
            Physics2D.IgnoreCollision(other.collider, boxCollider2d);
        }
        
        if (other.gameObject.CompareTag("Platform"))
        {
            Physics2D.IgnoreCollision(other.collider, boxCollider2d);
        }
        
    }
}



