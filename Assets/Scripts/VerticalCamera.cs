﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalCamera : MonoBehaviour
{
    public float endTilePos;
    public void FollowPlayer(GameObject player)
    {
        if (player.transform.position.y < endTilePos)
        {
            Vector3 temp = new Vector3(transform.position.x, player.transform.position.y, -10);
            if (player.transform.position.y > 0)
            {
                transform.position = temp;
            }
            else
            {
                transform.position = new Vector3(0, 0, -10);
            }
        }
    }
}
