
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
                Debug.LogFormat("WEASPOT {0}, playerY : {1} vs {0} : {2}", transform.parent.parent.name, collision.transform.position.y, transform.parent.parent.position.y);
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
