using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AStarPathfinding : MonoBehaviour
{    
    public List<AStarNode> openList;   // lista de nós abertos que são candidatos a fazer parte do caminho    
    public List<AStarNode> closedList; // lista de nós fechados que já foram visitados    
    public List<AStarNode> path;       // lista de nós que formam o caminho    
    public List<AStarNode> grid;       // lista de todos os nós do grid
    public AStarNode startNode;        // nó inicial
    public AStarNode endNode;          // nó final
    public AStarNode currentNode;      // nó atual que está sendo avaliado
    // Materiais para indicar o status dos nós
    public Material openMaterial, closedMaterial, pathMaterial, gridMaterial, startMaterial, endMaterial;
    public enum GameStatus { None, SelectStart, SelectEnd, Ready };
    public GameStatus gameStatus = GameStatus.None; // status do jogo
    public Toggle toggle; // toggle para visualizar passo a passo
    public TMPro.TextMeshProUGUI text; // texto para instruções
    
    void Start()
    {
        // captura todos os nós do grid
        grid = new List<AStarNode>(FindObjectsOfType<AStarNode>());
        text.text = "Selecione o nó inicial e o nó final entre os nós brancos. Se desejar visualizar passo a passo, marque referida caixa.";
    }

    public void SelectStartNode()
    {
        if(gameStatus != GameStatus.None) return;
        gameStatus = GameStatus.SelectStart;
        text.text = "Selecione o nó inicial entre os nós brancos.";
    }     

    public void SetStartNode(AStarNode node)
    {
        if(startNode != null)
        {
            startNode.SetMaterial(gridMaterial);
        }
        startNode = node;
        node.SetMaterial(startMaterial);
        gameStatus = GameStatus.SelectEnd;
        text.text = "Selecione o nó final entre os nós brancos.";
    }

    public void SetEndNode(AStarNode node)
    {
        if (endNode != null)
        {
            endNode.SetMaterial(gridMaterial);
        }
        endNode = node;
        node.SetMaterial(endMaterial);
        gameStatus = GameStatus.Ready;
        text.text = "Clique em calcular rota.";
    }

    public void StartAStar()
    {
        // Reseta o caminho
        foreach (AStarNode node in grid)
        {
            if (node.status != NodeStatus.Obstacle)
            {
                node.SetMaterial(gridMaterial);
                node.ResetNode();
            }
        }
        // Verifica se os nós de início e fim foram definidos
        if (startNode == null || endNode == null || gameStatus != GameStatus.Ready)
        {
            text.text = "Os nós iniciais e finais não foram definidos";
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

        if (toggle.isOn)
        {
            InvokeRepeating("CalculatePath", 0, 0.5f);
        } else
        {
            CalculatePath();
        }
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
        // Remove o nó atual da lista aberta
        openList.Remove(currentNode);
        // Adiciona o nó atual à lista fechada
        closedList.Add(currentNode);
        // Muda o material do nó atual para indicar que ele está na lista fechada
        currentNode.SetMaterial(closedMaterial);
        // Se a lista aberta estiver vazia, não há caminho
        if (openList.Count == 0)
        {
            text.text = "Não há caminho";
            CancelInvoke("CalculatePath");
            gameStatus = GameStatus.None;
            return;
        }
        // Ordena a lista aberta pelo custo F
        openList.Sort((node1, node2) => node1.fCost.CompareTo(node2.fCost));

        // Verifica se o nó atual é o nó final
        if (currentNode == endNode)
        {
            if(toggle.isOn)
            {
                CancelInvoke("CalculatePath");
            }
            SetPath(currentNode);
        }
        else
        {
            // Define o nó atual como o nó com menor custo F
            currentNode = openList[0];
            // Chama a função novamente
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
        gameStatus = GameStatus.None;
    }
}
