using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class AStarPathfinding : MonoBehaviour
{
    public List<AStarNode> openList;    // nos abertos - canditados
    public List<AStarNode> closedList;  // nos fechados - visitados
    public List<AStarNode> path;        // para reconstrucao do caminho
    public List<AStarNode> grid;        // todos os nos
    public AStarNode startNode;         // origem
    public AStarNode endNode;           // destino
    public AStarNode currentNode;       // no em avaliacao
    // materiais para indicar estados dos nos
    public Material openMaterial, closedMaterial, pathMaterial, gridMaterial, startMaterial, endMaterial;
    public enum GameStatus { None, SelectStart, SelectEnd, Ready };
    public GameStatus gameStatus = GameStatus.None;  // estado do jogo
    public Toggle toggle;  // para ligar o passo a passo
    public TMPro.TextMeshProUGUI text;  // texto de instrucoes

    void Start()
    {
        grid = new List<AStarNode>(FindObjectsByType<AStarNode>(FindObjectsSortMode.None));
        text.text = "Selecione o nó inicial e o nó final entre os nós brancos. Para visualizar passo a passo, marque a referida caixa.";
    }

    public void SelectStartNode()
    {
        if (gameStatus != GameStatus.None) return;

        gameStatus = GameStatus.SelectStart;
        text.text = "Clique no nó inicial para iniciar o caminho.";
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
        // reseta o caminho
        foreach (AStarNode node in grid)
        {
            if (node.status != NodeStatus.Obstacle)
            {
                node.SetMaterial(gridMaterial);
                node.ResetNode();
            }
        }
        // verifica se os nos de inicio e fim foram definidos
        if (startNode == null || endNode == null || gameStatus != GameStatus.Ready)
        {
            text.text = "Os nós inicial e final não foram definidos.";
            return;
        }

        currentNode = startNode;
        // inicializa as listas
        openList = new List<AStarNode>();
        closedList = new List<AStarNode>();
        path = new List<AStarNode>();
        // avalia o no atual
        openList.Add(currentNode);
        currentNode.CalculateCost(startNode, endNode);

        if (toggle.isOn) // calcula em intervalos
        {
            InvokeRepeating("CalculatePath", 0f, 0.5f);
        }
        else
        {
            CalculatePath(); // calcula sem intervalos
        }
    }

    void CalculatePath()
    {
        foreach (AStarNode node in grid)
        {
            float deltaX = Mathf.Abs(node.transform.position.x - currentNode.transform.position.x);
            float deltaZ = Mathf.Abs(node.transform.position.z - currentNode.transform.position.z);

            if (Mathf.Approximately(deltaX + deltaZ, 1f))
            {
                if (closedList.Contains(node) || node.status == NodeStatus.Obstacle || node == startNode)
                {
                    continue;
                }

                node.CalculateCost(startNode, endNode); // calcula g e h do vizinho
                node.parent = currentNode;              // seta o no atual como pai dos vizinho
                openList.Add(node);                     // adiciona o vizinho a lista aberta
                node.SetMaterial(openMaterial);         // muda o material para lista aberta
            }
        }
        openList.Remove(currentNode);            // remove o atual da lista aberta
        closedList.Add(currentNode);             // adiciona o atual na lista fechada
        currentNode.SetMaterial(closedMaterial); // muda o material para lista fechada
        // se a lista aberta esta vazia, nao existe caminho
        if (openList.Count == 0)
        {
            text.text = "Não foi possível encontrar um caminho.";
            CancelInvoke("CalculatePath");
            gameStatus = GameStatus.None;
            return;
        }

        // ordena a lista aberta pelo custo f
        openList.Sort((node1, node2) => node1.fCost.CompareTo(node2.fCost));
        // verifica se o no avaliado e o no final
        if (currentNode == endNode) {
            if (toggle.isOn) CancelInvoke("CalculatePath");
            SetPath(currentNode); // reconstroi o caminho
        }
        else {
            // define o no avaliado como o no com menor custo f
            currentNode = openList[0];
            // chama a funcao novamente
            if (!toggle.isOn) CalculatePath();
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
}
