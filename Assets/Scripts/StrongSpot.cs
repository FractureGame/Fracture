using Photon.Pun;
using UnityEngine;

public class StrongSpot : MonoBehaviourPunCallbacks
{

    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        
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
}
