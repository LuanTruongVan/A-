
using System;
using System.Collections.Generic;
using UnityEngine;

public class Grip : MonoBehaviour
{
    public LayerMask unWalkLayer;
    public Vector2 gripWorldSize;
    public float nodeRadius; 
    private Node[,] grip;
    private float nodeDiameter;
    private int gripSizeX, gripSizeY;
    public int MaxSize => gripSizeX * gripSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gripSizeX = Mathf.RoundToInt(gripWorldSize.x / nodeDiameter);
        gripSizeY = Mathf.RoundToInt(gripWorldSize.y / nodeDiameter);
        CreateGrip();
    }

    private void CreateGrip()
    {
        grip = new Node[gripSizeX, gripSizeY];
        Vector3 worldBottomLeft =
            transform.position - Vector3.right * gripWorldSize.x / 2 - Vector3.forward * gripWorldSize.y/2;
        for (int x = 0; x < gripSizeX; x++)
        {
            for (int y = 0; y < gripSizeY; y++)
            {
                
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius)+Vector3.forward*(y*nodeDiameter+nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius,unWalkLayer);
                grip[x, y] = new Node(walkable, worldPoint,x,y);
            }
        }
    }

    public Node NodeFromWorld(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gripWorldSize.x / 2) / gripSizeX;
        float percentY = (worldPosition.z + gripWorldSize.y / 2) / gripSizeY;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gripSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gripSizeY - 1) * percentY);
        return grip[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighboursNode = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.gripX + x;
                int checkY = node.gripY + y;
                if (checkX >= 0 && checkX < gripSizeX && checkY >= 0 && checkY < gripSizeY)
                {
                    neighboursNode.Add(grip[checkX,checkY]);
                }
            }
        }
        return neighboursNode;
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position,new Vector3(gripWorldSize.x,1,gripWorldSize.y));
        if (grip != null)
        {
            
            foreach (Node node in grip)
            {
                Gizmos.color = node.isWalk ? Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition,Vector3.one*(nodeDiameter-0.1f));
            }
        }
    }
}
