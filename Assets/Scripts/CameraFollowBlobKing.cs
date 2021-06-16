using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowBlobKing : MonoBehaviour
{
    private Vector3 blobPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        blobPos = GameObject.Find("RoiBlob").transform.position;
        transform.position = blobPos;
    }
}
