using UnityEngine;
using UnityEngine.Tilemaps;

public class DestroyChecker : MonoBehaviour
{
    
    public Activation topDestroy;
    public Activation botDestroy;

    public Tilemap tilemap;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (topDestroy.isActivated && botDestroy.isActivated)
        {
            GameObject grid = GameObject.Find("Grid");
            Debug.Log(grid.transform.Find(tilemap.name).gameObject.name);
            Destroy(grid.transform.Find(tilemap.name).gameObject);
        }
    }
}
