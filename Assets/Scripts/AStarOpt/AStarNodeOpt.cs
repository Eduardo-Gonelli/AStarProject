using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AStarNodeOpt : MonoBehaviour
{
    public AStarNodeOpt parent;
    //public List<AStarNode> neighbors;
    public float gCost, hCost, fCost;    
    public NodeStatus status;
    AStarPathfindingOpt pathfinding;
    TextMeshPro text;
    private void Start()
    {
        pathfinding = FindObjectOfType<AStarPathfindingOpt>();
        text = GetComponentInChildren<TextMeshPro>();
    }

    // Não calculamos o custo para todo nó, apenas para os nós que estão na lista
    // aberta e possuem o menor custo f
    public void CalculateCost(AStarNodeOpt startNode, AStarNodeOpt endNode, float parentCost)
    {
        gCost = parentCost + 1;
        Vector3 dir = endNode.transform.position - transform.position;
        hCost = dir.sqrMagnitude;
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
        if (pathfinding.gameStatus == AStarPathfindingOpt.GameStatus.SelectStart)
        {
            pathfinding.SetStartNode(this);            
        }
        else if(pathfinding.gameStatus == AStarPathfindingOpt.GameStatus.SelectEnd)
        {
            pathfinding.SetEndNode(this);            
        }
        else { return; }
    }
}
