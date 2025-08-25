using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class AStarPathfinding : MonoBehaviour
{
    public List<AStarNode> openList;
    public List<AStarNode> closedList;
    public List<AStarNode> path;
    public List<AStarNode> grid;
    public AStarNode startNode;
    public AStarNode endNode;
    public AStarNode currentNode;
    public Material openMaterial, closedMaterial, pathMaterial, gridMaterial, startMaterial, endMaterial;
    public enum GameStatus { None, SelectStart, SelectEnd, Ready };
    public GameStatus gameStatus = GameStatus.None;
    public Toggle toggle;
    public TMPro.TextMeshProUGUI text;

    void Start()
    {
        grid = new List<AStarNode>(FindObjectsByType<AStarNode>(FindObjectsSortMode.None));
        text.text = "Selecione o nó inicial e o nó final entre os nós brancos. Se desenar visualizar passo a passo, marque a referida caixa.";
    }

    public void SelectStartNode()
    {
        if (gameStatus != GameStatus.None) return;

        gameStatus = GameStatus.SelectStart;
        text.text = "Clique no nó inicial (branco) para iniciar o caminho.";
    }

    public void SetStartNode(AStarNode node)
    {
        if (startNode != null) startNode.SetMaterial(gridMaterial);

        startNode = node;
        node.SetMaterial(startMaterial);
        gameStatus = GameStatus.SelectEnd;
        text.text = "Clique no nó final (branco) para finalizar o caminho.";
    }

    public void SetEndNode(AStarNode node)
    {
        if (endNode != null) endNode.SetMaterial(gridMaterial);
        endNode = node;
        node.SetMaterial(endMaterial);
        gameStatus = GameStatus.Ready;
        text.text = "Clique calcular rota para iniciar o caminho.";
    }

    public void StartAStar()
    {
        foreach (AStarNode node in grid)
        {
            if (node.status != NodeStatus.Obstacle)
            {
                node.SetMaterial(gridMaterial);
                node.ResetNode();
            }
        }

        if (startNode == null || endNode == null || gameStatus != GameStatus.Ready)
        {
            text.text = "Os nós inicial e final não foram definidos.";
            return;
        }

        currentNode = startNode;
        openList = new List<AStarNode>();
        closedList = new List<AStarNode>();
        path = new List<AStarNode>();

        openList.Add(currentNode);
        currentNode.CalculateCost(startNode, endNode);

        if (toggle.isOn)
        {
            InvokeRepeating("CalculatePath", 0f, 0.5f);
        }
        else
        {
            CalculatePath();
        }
    }

    void CalculatePath()
    {
        foreach (AStarNode node in grid)
        {
            if ((Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x + 1)
                && Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z))
                || (Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x - 1)
                && Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z))
                || (Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x)
                && Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z + 1))
                || (Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x)
                && Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z - 1)))
            {
                if (closedList.Contains(node) || node.status == NodeStatus.Obstacle || node == startNode)
                {
                    continue;
                }

                node.CalculateCost(startNode, endNode);
                node.parent = currentNode;
                openList.Add(node);
                node.SetMaterial(openMaterial);
            }
        }
        openList.Remove(currentNode);
        closedList.Add(currentNode);
        currentNode.SetMaterial(closedMaterial);

        if (openList.Count == 0)
        {
            text.text = "Não foi possível encontrar um caminho.";
            CancelInvoke("CalculatePath");
            gameStatus = GameStatus.None;
            return;
        }

        openList.Sort((node1, node2) => node1.fCost.CompareTo(node2.fCost));

        if (currentNode == endNode)
        {
            if (toggle.isOn)
            {
                CancelInvoke("CalculatePath");
            }
            SetPath(currentNode);
        }
        else
        {
            currentNode = openList[0];
            if (!toggle.isOn)
            {
                CalculatePath();
            }
        }
    }

    void SetPath(AStarNode lastNode)
    {
        foreach (AStarNode node in grid)
        {
            if (node.status != NodeStatus.Obstacle && node != startNode && node != endNode)
            {
                node.SetMaterial(gridMaterial);
            }
        }

        path.Add(lastNode);
        while (lastNode != startNode)
        {
            lastNode = lastNode.parent;
            path.Add(lastNode);
        }

        path.Reverse();

        foreach (AStarNode node in path)
        {
            if (node != startNode && node != endNode)
            {
                node.SetMaterial(pathMaterial);
            }
        }
        gameStatus = GameStatus.None;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
