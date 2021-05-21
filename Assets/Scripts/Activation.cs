using UnityEngine;

public class Activation : MonoBehaviour
{
    public bool isActivated = false;
    // Start is called before the first frame update

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D other)
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
