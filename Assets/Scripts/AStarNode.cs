using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AStarNode : MonoBehaviour
{
    public AStarNode parent;
    public List<AStarNode> neighbors;
    public float gCost, hCost, fCost;    
    public NodeStatus status;
    AStarPathfinding pathfinding;
    TextMeshPro text;
    private void Start()
    {
        pathfinding = FindObjectOfType<AStarPathfinding>();
        text = GetComponentInChildren<TextMeshPro>();
    }

    // N�o calculamos o custo para todo n�, apenas para os n�s que est�o na lista
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
    
    // Podemos setar o material do n� para indicar o status dele
    public void SetMaterial(Material material)
    {        
        GetComponent<Renderer>().material = material;
    }

    private void OnMouseDown()
    {
        if(pathfinding.gameStatus == AStarPathfinding.GameStatus.SelectStart)
        {
            pathfinding.SetStartNode(this);
        }
        else if(pathfinding.gameStatus == AStarPathfinding.GameStatus.SelectEnd)
        {
            pathfinding.SetEndNode(this);
            pathfinding.gameStatus = AStarPathfinding.GameStatus.Ready;
        }
        else
        {
            return;
        }
    }
}
