using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform target;
    private float speed=1;
    private Vector3[] path;
    private int targetIndex;

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position,target.position,OnPathFound);
    }

    public void OnPathFound(Vector3[] newpath,bool isSuccess)
    {
        if (isSuccess)
        {
            path = newpath;
        }

        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");

    }

    IEnumerator FollowPath()
    {
        Vector3 currentWayPoint=path[0];
        while (true)
        {
            if (transform.position == currentWayPoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }

                currentWayPoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWayPoint, speed*Time.deltaTime);
            yield return null; 
        }
    }
}
