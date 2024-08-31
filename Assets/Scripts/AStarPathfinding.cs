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
        // captura todos os nós do grid
        grid = new List<AStarNode>(FindObjectsOfType<AStarNode>());
    }

    public void StartAStar()
    {
        // Verifica se os nós de início e fim foram definidos
        if (startNode == null || endNode == null)
        {
            Debug.LogError("Os nós iniciais e finais não foram definidos");
            return;
        }

        // Inicializa as listas
        openList = new List<AStarNode>();
        closedList = new List<AStarNode>();
        path = new List<AStarNode>();
        currentNode = startNode;

        // Adiciona o nó inicial à lista aberta
        openList.Add(currentNode);
        // Calcula os custos g e h do nó inicial
        currentNode.CalculateCost(startNode, endNode);  
        // Chama a função para calcular o caminho
        CalculatePath(currentNode);
    }

    void CalculatePath(AStarNode currentNode)
    {
        // Captura os vizinhos do nó atual
        foreach (AStarNode node in grid)
        {
            // Verifica quem são os vizinhos pelo transform position x e z
            // Aqui estamos considerando apenas os vizinhos na horizontal e vertical
            // Mathf.Approximately é usado para comparar floats e evitar erros de precisão
            if((Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x + 1) || Mathf.Approximately(node.transform.position.x, currentNode.transform.position.x -1)) && (Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z + 1) || Mathf.Approximately(node.transform.position.z, currentNode.transform.position.z - 1)))
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

            }
        }
        // Se a lista aberta estiver vazia, não há caminho
        if (openList.Count == 0)
        {
            Debug.LogError("Não há caminho");
            return;
        }
        // Ordena a lista aberta pelo custo F
        openList.Sort((node1, node2) => node1.FCost.CompareTo(node2.FCost));
        // Remove o nó atual da lista aberta
        openList.Remove(currentNode);
        // Adiciona o nó atual à lista fechada
        closedList.Add(currentNode);
        
        // Verifica se o nó atual é o nó final
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
        // Define o nó atual como o nó com menor custo F
        currentNode = openList[0];
        // Chama a função novamente
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
