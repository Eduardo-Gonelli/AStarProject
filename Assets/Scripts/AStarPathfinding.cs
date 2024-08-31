using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    public List<AStarNode> openList;
    public List<AStarNode> closedList;
    public List<AStarNode> path;
    public List<AStarNode> grid;
    public AStarNode startNode;
    public AStarNode endNode;
    public AStarNode currentNode;
    public Material openMaterial;
    public Material closedMaterial;
    public Material pathMaterial;
    public Material gridMaterial;    

    // Start is called before the first frame update
    void Start()
    {
        // captura todos os n�s do grid
        grid = new List<AStarNode>(FindObjectsOfType<AStarNode>());
    }

    public void StartAStar()
    {
        // Verifica se os n�s de in�cio e fim foram definidos
        if (startNode == null || endNode == null)
        {
            Debug.LogError("Os n�s iniciais e finais n�o foram definidos");
            return;
        }

        // Inicializa as listas
        openList = new List<AStarNode>();
        closedList = new List<AStarNode>();
        path = new List<AStarNode>();
        currentNode = startNode;

        // Adiciona o n� inicial � lista aberta
        openList.Add(currentNode);
        // Calcula os custos g e h do n� inicial
        currentNode.CalculateCost(startNode, endNode);  
        // Chama a fun��o para calcular o caminho
        CalculatePath(currentNode);
    }

    void CalculatePath(AStarNode currentNode)
    {
        // Captura os vizinhos do n� atual
        foreach (AStarNode node in grid)
        {
            // Verifica quem s�o os vizinhos pelo transform position x e z
            // Aqui estamos considerando apenas os vizinhos na horizontal e vertical
            // Mathf.Approximately � usado para comparar floats e evitar erros de precis�o
            if((Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x + 1) || Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x -1)) && (Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z + 1) || Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z - 1)))
            {
                // Verifica se o vizinho est� na lista fechada ou se � um obst�culo ou se � o n� inicial
                if (closedList.Contains(node) || node.status == NodeStatus.Obstacle || node == startNode)
                {
                    continue;
                }
                // Calcula os custos g e h do vizinho
                node.CalculateCost(startNode, endNode);
                // Adiciona o n� atual como pai do vizinho
                node.parent = currentNode;
                // Adiciona o vizinho � lista aberta
                openList.Add(node);
                // muda o material do vizinho para indicar que ele est� na lista aberta

            }
        }
        // Se a lista aberta estiver vazia, n�o h� caminho
        if (openList.Count == 0)
        {
            Debug.LogError("N�o h� caminho");
            return;
        }
        // Ordena a lista aberta pelo custo F
        openList.Sort((node1, node2) => node1.FCost.CompareTo(node2.FCost));
        // Remove o n� atual da lista aberta
        openList.Remove(currentNode);
        // Adiciona o n� atual � lista fechada
        closedList.Add(currentNode);
        
        // Verifica se o n� atual � o n� final
        if (currentNode == endNode)
        {
            // Se for, calcula o caminho
            while (currentNode != startNode)
            {
                currentNode = currentNode.parent;
                path.Add(currentNode);
            }
            // Inverte o caminho
            path.Reverse();
            return;
        }
        // Define o n� atual como o n� com menor custo F
        currentNode = openList[0];
        // Chama a fun��o novamente
        CalculatePath(currentNode);
    }

    public void SetStartNode(AStarNode node)
    {
        startNode = node;
    }

    public void SetEndNode(AStarNode node)
    {
        endNode = node;
    }
}
