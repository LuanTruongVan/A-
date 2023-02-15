using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public PathRequestManager pathRequestManager;
    public Transform seeker, target;
    private Grip _grip;

    private void Awake()
    {
        pathRequestManager = GetComponent<PathRequestManager>();
        _grip = GetComponent<Grip>();
    }

    public void StartFindPath(Vector3 pathStart, Vector3 pathEnd)
    {
        StartCoroutine(FindPath(pathStart, pathEnd));
    }
    IEnumerator  FindPath(Vector3 startPos, Vector3 tarPos)
    {
        
        Node startNode = _grip.NodeFromWorld(startPos);
        Node targetNode = _grip.NodeFromWorld(tarPos);
        Vector3[] wayPoints = new Vector3[0];
        bool pathSuccess = false;
        if (startNode.isWalk && targetNode.isWalk)
        {
            Heap<Node> openSet = new Heap<Node>(_grip.MaxSize);
            HashSet<Node> closeSet = new HashSet<Node>();
            openSet.Add(startNode);
            while (openSet.Count>0)
            {
                Node node = openSet.RemoveFirst();
                closeSet.Add(node);
                if (node == targetNode)
                {
                    pathSuccess = true;
                    print("Success finding");
                    break;
                }
                foreach (Node neighbour in _grip.GetNeighbours(node))
                {
                    if(!neighbour.isWalk|| closeSet.Contains(neighbour)){continue;}

                    int newMovementCostToNeighbours = node.gCost + GetDistance(node, neighbour);
                    if (newMovementCostToNeighbours < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbours;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = node;
                        if (!openSet.Contains(neighbour))
                        {       
                            openSet.Add(neighbour);
                        }
                    }

                }
            
            }
        }
        

        yield return null;
        if (pathSuccess)
        {
            wayPoints = RetracePath(startNode, targetNode);
        }
        pathRequestManager.FinishProcessingPath(wayPoints,pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode!=startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] wayPoints = SimplePath(path);
        Array.Reverse(wayPoints);
        return wayPoints;
    }

    Vector3[] SimplePath(List<Node> path)
    {
        List<Vector3> wayPoints = new List<Vector3>();
        Vector2 directionOld=Vector2.zero;
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 direction = new Vector2(path[i - 1].gripX - path[i].gripX, path[i - 1].gripY - path[i].gripY);
            if (direction != directionOld)
            {
                wayPoints.Add(path[i].worldPosition);
            }

            directionOld = direction;
        }

        return wayPoints.ToArray();
    }
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gripX - nodeB.gripX);
        int dstY = Mathf.Abs(nodeA.gripY - nodeB.gripY);
        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }

   
}
