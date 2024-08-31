using System.Collections.Generic;
using UnityEngine;

public class AStarNode : MonoBehaviour
{
    public List<AStarNode> neighbors;
    public float gCost;
    public float hCost;
    public float FCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    public AStarNode parent;
    public NodeStatus status;    
    // Start is called before the first frame update
    void Start()
    {        
        gCost = Mathf.Infinity;
        hCost = Mathf.Infinity;        
    }

    public void SetMaterial(Material material)
    {        
        GetComponent<Renderer>().material = material;
    }
    
    public void CalculateCost(AStarNode startNode, AStarNode endNode)
    {
        gCost = Vector3.Distance(transform.position, startNode.transform.position);
        hCost = Vector3.Distance(transform.position, endNode.transform.position);
    }
}
