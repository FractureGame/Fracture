using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowKing : MonoBehaviour
{
    private Vector3 blobPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        blobPos = GameObject.Find("RoiBlob").transform.Find("Graphics").transform.position;
        transform.position = new Vector3(blobPos.x, blobPos.y, -10);
    }
}
