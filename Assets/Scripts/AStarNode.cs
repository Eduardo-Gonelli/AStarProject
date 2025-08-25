using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class AStarNode : MonoBehaviour
{
    public AStarNode parent;
    public List<AStarNode> neighbors;
    public float gCost, hCost, fCost;
    public NodeStatus status;
    AStarPathfinding pathfinding;
    TextMeshPro text;
    void Start()
    {
        pathfinding = FindFirstObjectByType<AStarPathfinding>();
        text = GetComponentInChildren<TextMeshPro>();
    }

    // O custo é calculado para todo o nó da lista aberta que possui o menor custo f
    public void CalculateCost(AStarNode startNode, AStarNode endNode)
    {
        gCost = Vector3.Distance(transform.position, startNode.transform.position);
        hCost = Vector3.Distance(transform.position, endNode.transform.position);
        fCost = gCost + hCost;
        // gCost:F2 é o mesmo que escrever gCost.ToString("F2")
        text.text = $"G: {gCost:F2}\nH: {hCost:F2}\nF: {fCost:F2}";
    }

    public void SetMaterial(Material material)
    {
        GetComponent<Renderer>().material = material;
    }

    public void ResetNode()
    {
        parent = null;
        gCost = hCost = fCost = 0f;
        text.text = string.Empty;
        neighbors.Clear();
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
