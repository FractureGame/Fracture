
using Photon.Pun;
using UnityEngine;

public class WeakSpot : MonoBehaviour
{
    public GameObject objectToDestroy;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (transform.parent.parent.name.StartsWith("Harpie"))
            {
                Debug.Log(transform.parent.parent.name);
                if (collision.transform.position.y > transform.parent.parent.position.y)
                    PhotonNetwork.Destroy(objectToDestroy);
            }
            else
            {
                PhotonNetwork.Destroy(objectToDestroy);
            }
        }
    }
}
