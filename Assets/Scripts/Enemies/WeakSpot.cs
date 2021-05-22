
using Photon.Pun;
using UnityEngine;

public class WeakSpot : MonoBehaviour
{
    public GameObject objectToDestroy;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (transform.parent.name.StartsWith("Harpie"))
            {
                if (collision.transform.position.y > transform.parent.position.y)
                    PhotonNetwork.Destroy(objectToDestroy);
            }
            else
            {
                PhotonNetwork.Destroy(objectToDestroy);
            }
        }
    }
}
