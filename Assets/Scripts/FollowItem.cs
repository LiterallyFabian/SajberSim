using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowItem : MonoBehaviour
{
    private Vector3 itemPosition;
    public GameObject followingItem;
    bool start = false;

    void Update()
    {

        itemPosition = Camera.main.ScreenToWorldPoint(transform.position);
        if(start)
        followingItem.transform.position = new Vector3(itemPosition.x, itemPosition.y - 0.07f, -1);
        start = true;
    }
}