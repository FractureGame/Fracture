using System;
using UnityEngine;

public class Activation : MonoBehaviour
{
    public bool isActivated = false;

    private BoxCollider2D boxCollider2d;
    // Start is called before the first frame update

    private void Start()
    {
        boxCollider2d = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame

    // private void Update()
    // {
    //     if (boxCollider2d.IsTouchingLayers(13))
    //     {
    //         isActivated = true;
    //     }
    //     else
    //     {
    //         isActivated = false;
    //     }
    // }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isActivated = true;
        }
    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isActivated = true;
        }
    }
    
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isActivated = false;
        }
    }
}
