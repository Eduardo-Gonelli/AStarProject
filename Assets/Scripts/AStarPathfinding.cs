using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    // openList: lista de nós abertos que são candidatos a fazer parte do caminho
    public List<AStarNode> openList;
    // closedList: lista de nós fechados que já foram visitados
    public List<AStarNode> closedList;
    // path: lista de nós que formam o caminho
    public List<AStarNode> path;
    // grid: lista de todos os nós do grid
    public List<AStarNode> grid;
    public AStarNode startNode;   // nó inicial
    public AStarNode endNode;     // nó final
    public AStarNode currentNode; // nó atual
    // Materiais para indicar o status dos nós
    public Material openMaterial, closedMaterial, pathMaterial, gridMaterial, startMaterial, endMaterial;
    public enum GameStatus { None, SelectStart, SelectEnd, Ready };
    public GameStatus gameStatus = GameStatus.None;

    // Start is called before the first frame update
    void Start()
    {
        // captura todos os nós do grid
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
        // Verifica se os nós de início e fim foram definidos
        if (startNode == null || endNode == null || gameStatus != GameStatus.Ready)
        {
            Debug.LogError("Os nós iniciais e finais não foram definidos");
            return;
        }
        currentNode = startNode;
        // Inicializa as listas
        openList = new List<AStarNode>();
        closedList = new List<AStarNode>();
        path = new List<AStarNode>();
        // Chama a função para calcular o caminho
        // Adiciona o nó inicial à lista aberta
        openList.Add(currentNode);
        // Calcula os custos g e h do nó inicial
        currentNode.CalculateCost(startNode, endNode);

        CalculatePath();
    }

    void CalculatePath()
    {

        
        // Captura os vizinhos do nó atual
        foreach (AStarNode node in grid)
        {
            // Verifica quem são os vizinhos pelo transform position x e z
            // Aqui estamos considerando apenas os vizinhos na horizontal e vertical
            // Mathf.Approximately é usado para comparar floats e evitar erros de precisão
            if ((Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x + 1)
                && Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z)) 
                || (Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x - 1) 
                && Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z)) 
                || (Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z + 1) 
                && Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x)) 
                || (Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z - 1) 
                && Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x)))
            {
                // Verifica se o vizinho está na lista fechada ou se é um obstáculo ou se é o nó inicial
                if (closedList.Contains(node) || node.status == NodeStatus.Obstacle || node == startNode)
                {
                    continue;
                }
                // Calcula os custos g e h do vizinho
                node.CalculateCost(startNode, endNode);
                // Adiciona o nó atual como pai do vizinho
                node.parent = currentNode;
                // Adiciona o vizinho à lista aberta
                openList.Add(node);
                // muda o material do vizinho para indicar que ele está na lista aberta
                node.SetMaterial(openMaterial);
            }
        }
        // Se a lista aberta estiver vazia, não há caminho
        if (openList.Count == 0)
        {
            Debug.LogError("Não há caminho");
            return;
        }
        // Ordena a lista aberta pelo custo F
        openList.Sort((node1, node2) => node1.fCost.CompareTo(node2.fCost));
        // Remove o nó atual da lista aberta
        openList.Remove(currentNode);
        // Adiciona o nó atual à lista fechada
        closedList.Add(currentNode);
        // Muda o material do nó atual para indicar que ele está na lista fechada
        currentNode.SetMaterial(closedMaterial);

        // Verifica se o nó atual é o nó final
        if (currentNode == endNode)
        {
            SetPath(currentNode);
        }
        else
        {
            // Define o nó atual como o nó com menor custo F
            currentNode = openList[0];
            // Chama a função novamente
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

        // Muda o material dos nós do caminho
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
