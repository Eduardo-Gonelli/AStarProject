using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AStarNode : MonoBehaviour
{
    public AStarNode parent;
    //public List<AStarNode> neighbors;
    public float gCost, hCost, fCost;    
    public NodeStatus status;
    AStarPathfinding pathfinding;
    TextMeshPro text;
    private void Start()
    {
        pathfinding = FindObjectOfType<AStarPathfinding>();
        text = GetComponentInChildren<TextMeshPro>();
    }

    // Não calculamos o custo para todo nó, apenas para os nós que estão na lista
    // aberta e possuem o menor custo f
    public void CalculateCost(AStarNode startNode, AStarNode endNode)
    {
        gCost = Vector3.Distance(transform.position, startNode.transform.position);
        hCost = Vector3.Distance(transform.position, endNode.transform.position);
        fCost = gCost + hCost;
        text.text = $"G: {gCost.ToString("00.00")} \n " +
            $"H: {hCost.ToString("00.00")} \n" +
            $"F: {fCost.ToString("00.00")}";
    }
    
    // Podemos setar o material do nó para indicar o status dele
    public void SetMaterial(Material material)
    {        
        GetComponent<Renderer>().material = material;
    }

    public void ResetNode()
    {
        parent = null;
        gCost = hCost = fCost = 0;
        text.text = "";
        //neighbors.Clear();
    }

    private void OnMouseDown()
    {
        if (status == NodeStatus.Obstacle) return;        
        if (pathfinding.gameStatus == AStarPathfinding.GameStatus.SelectStart)
        {
            pathfinding.SetStartNode(this);            
        }
        else if(pathfinding.gameStatus == AStarPathfinding.GameStatus.SelectEnd)
        {
            pathfinding.SetEndNode(this);            
        }
        else { return; }
    }
}
