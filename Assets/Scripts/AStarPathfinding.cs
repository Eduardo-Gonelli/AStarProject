using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    // openList: lista de n�s abertos que s�o candidatos a fazer parte do caminho
    public List<AStarNode> openList;
    // closedList: lista de n�s fechados que j� foram visitados
    public List<AStarNode> closedList;
    // path: lista de n�s que formam o caminho
    public List<AStarNode> path;
    // grid: lista de todos os n�s do grid
    public List<AStarNode> grid;
    public AStarNode startNode;   // n� inicial
    public AStarNode endNode;     // n� final
    public AStarNode currentNode; // n� atual
    // Materiais para indicar o status dos n�s
    public Material openMaterial, closedMaterial, pathMaterial, gridMaterial, startMaterial, endMaterial;
    public enum GameStatus { None, SelectStart, SelectEnd, Ready };
    public GameStatus gameStatus = GameStatus.None;

    // Start is called before the first frame update
    void Start()
    {
        // captura todos os n�s do grid
        grid = new List<AStarNode>(FindObjectsOfType<AStarNode>());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartAStar();
        }
    }

    public void StartAStar()
    {
        // Verifica se os n�s de in�cio e fim foram definidos
        if (startNode == null || endNode == null || gameStatus != GameStatus.Ready)
        {
            Debug.LogError("Os n�s iniciais e finais n�o foram definidos");
            return;
        }
        currentNode = startNode;
        // Inicializa as listas
        openList = new List<AStarNode>();
        closedList = new List<AStarNode>();
        path = new List<AStarNode>();
        // Chama a fun��o para calcular o caminho
        // Adiciona o n� inicial � lista aberta
        openList.Add(currentNode);
        // Calcula os custos g e h do n� inicial
        currentNode.CalculateCost(startNode, endNode);

        CalculatePath();
    }

    void CalculatePath()
    {

        
        // Captura os vizinhos do n� atual
        foreach (AStarNode node in grid)
        {
            // Verifica quem s�o os vizinhos pelo transform position x e z
            // Aqui estamos considerando apenas os vizinhos na horizontal e vertical
            // Mathf.Approximately � usado para comparar floats e evitar erros de precis�o
            if ((Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x + 1)
                && Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z)) 
                || (Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x - 1) 
                && Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z)) 
                || (Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z + 1) 
                && Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x)) 
                || (Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z - 1) 
                && Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x)))
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
                node.SetMaterial(openMaterial);
            }
        }
        // Se a lista aberta estiver vazia, n�o h� caminho
        if (openList.Count == 0)
        {
            Debug.LogError("N�o h� caminho");
            return;
        }
        // Ordena a lista aberta pelo custo F
        openList.Sort((node1, node2) => node1.fCost.CompareTo(node2.fCost));
        // Remove o n� atual da lista aberta
        openList.Remove(currentNode);
        // Adiciona o n� atual � lista fechada
        closedList.Add(currentNode);
        // Muda o material do n� atual para indicar que ele est� na lista fechada
        currentNode.SetMaterial(closedMaterial);

        // Verifica se o n� atual � o n� final
        if (currentNode == endNode)
        {
            SetPath(currentNode);
        }
        else
        {
            // Define o n� atual como o n� com menor custo F
            currentNode = openList[0];
            // Chama a fun��o novamente
            CalculatePath();
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
        // Se for, calcula o caminho
        while (lastNode != startNode)
        {
            lastNode = lastNode.parent;
            path.Add(lastNode);
        }
        // Inverte o caminho
        path.Reverse();

        // Muda o material dos n�s do caminho
        foreach (AStarNode node in path)
        {
            if(node != startNode && node != endNode)
            {
                node.SetMaterial(pathMaterial);
            }            
        }
    }

    public void SetStartNode(AStarNode node)
    {
        if(startNode != null)
        {
            startNode.SetMaterial(gridMaterial);
        }
        startNode = node;
        node.SetMaterial(startMaterial);
    }

    public void SetEndNode(AStarNode node)
    {
        if (endNode != null)
        {
            endNode.SetMaterial(gridMaterial);
        }
        endNode = node;
        node.SetMaterial(endMaterial);
    }

    public void SelectStartNode()
    {
        gameStatus = GameStatus.SelectStart;
    }
    public void SelectEndNode()
    {
        gameStatus = GameStatus.SelectEnd;
    }    
}
