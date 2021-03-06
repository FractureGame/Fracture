﻿
using Photon.Pun;
using UnityEngine;

public class WeakSpot : MonoBehaviour
{
    public GameObject objectToDestroy;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Pieds"))
        {
            if (transform.parent.parent.name.StartsWith("Harpie"))
            {
                if (collision.transform.position.y > transform.parent.position.y)
                {
                    Destroy(objectToDestroy);
                    // PhotonNetwork.Destroy(objectToDestroy);
                }
                    
            }
            else
            {
                Destroy(objectToDestroy);
                PhotonNetwork.Destroy(objectToDestroy);
            }
        }
    }
}
