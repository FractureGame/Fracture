using System;
using System.Collections;
using System.Net;
using UnityEngine;
using Random = System.Random;

public class BlobKingAI : MonoBehaviour
{

    private GameObject playerTop;
    private GameObject playerBot;

    private Rigidbody2D rigidbody2d;
    private PolygonCollider2D polygonCollider2d;
    
    [Header("Actions")]
    private int nbActions = 1;
    private bool isActive;
    private int currentAction;
    private Coroutine coroutine;

    [Header("Jump")] private float jumpForce = 50;
    
    [Header("Layers")] 
    public LayerMask platformLayerMask;

    [Header("Collisions")] private bool isGrounded;
    
    // Start is called before the first frame update
    void Start()
    {
        playerTop = GameObject.Find("PlayerTop(Clone)");
        playerBot = GameObject.Find("PlayerBot(Clone)");

        rigidbody2d = GetComponent<Rigidbody2D>();
        polygonCollider2d = GetComponent<PolygonCollider2D>();
        // rigidbody2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        // isGrounded = IsGrounded();
        // if (isGrounded)
        //     Jump();
        // if (playerTop == null || playerBot == null)
        // {
        //     playerTop = GameObject.Find("PlayerTop(Clone)"); 
        //     playerBot = GameObject.Find("PlayerBot(Clone)");
        //     return;
        // }
        
        isGrounded = IsGrounded();
        if (!isActive && isGrounded)
        {
            currentAction = ChoseAction();
            isActive = true;
        }
        else
        {
            if (currentAction == 0)
            {
                // float playerPosX;
                // // Jump towards one player
                // // Choose player randomly
                // Random rand = new Random();
                // int proba = rand.Next(2);
                // if (proba == 0)
                // {
                //     playerPosX = playerTop.transform.position.x;
                // }
                // else
                // {
                //     playerPosX = playerBot.transform.position.x;
                // }

                if (!isGrounded)
                {
                    Jump();
                    isActive = false;
                }
                
            }
        }
    }

    private int ChoseAction()
    {
        Random rand = new Random();
        int proba = rand.Next(nbActions);
        return proba;
    }
    
    private bool IsGrounded()
    {
        // distance to the ground from which the player can jump again
        float extraHeightText = 0.1f;
        RaycastHit2D boxCastHit = Physics2D.BoxCast(polygonCollider2d.bounds.center, polygonCollider2d.bounds.size,0f,Vector2.down,
            extraHeightText, platformLayerMask);
        return boxCastHit.collider != null;
    }
    
    private void Jump()
    {
        Debug.Log("Jumping");
        rigidbody2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        // rigidbody2d.AddForce(new Vector2(playerPosX, 100), ForceMode2D.Impulse);
    }

    private void callBlobs()
    {
        
    }

    private void throwBlobs()
    {
        
    }
    
    
}
