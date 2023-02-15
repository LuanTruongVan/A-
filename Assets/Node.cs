using UnityEngine;
public class Node : IHeapItem<Node>
{
    public bool isWalk;
    public Vector3 worldPosition;
    public readonly int gripX;
    public readonly int gripY;
    public int gCost;
    public int hCost;
    public Node parent;
    private int _heapIndex;
    
    public Node(bool isWalk,Vector3 worldPosition,int gripX,int gripY)
    {
        this.isWalk = isWalk;
        this.worldPosition = worldPosition;
        this.gripX = gripX;
        this.gripY = gripY;
    }

    public int fCost => gCost + hCost;

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }

        return -compare;
    }

    public int HeapIndex { get=>_heapIndex; set=>_heapIndex=value; }
}
