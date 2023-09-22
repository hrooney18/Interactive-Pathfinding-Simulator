using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingScript : MonoBehaviour
{
    private Queue<Node> queue = new Queue<Node>();
    private Stack<Node> stack = new Stack<Node>();
    GameManagerScript gameManager;

    private List<Node> nodesToAnimate = new List<Node>();
    private List<Node> pathNodesToAnimate = new List<Node>();

    private bool isExploring = true;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManagerScript>();
    }

    public void ResetAndRunAlgorithm(string algorithm)
    {
        stack.Clear();
        queue.Clear();
        nodesToAnimate.Clear();
        pathNodesToAnimate.Clear();
        isExploring = true;
        foreach (Node node in GameManagerScript.nodes)
        {
            if (!gameManager.IsStartOrEndNode(node) && node.isActive())
                node.resetColor();
        }

        switch (algorithm)
        {
            case "BFS":
                BFS();
                break;
            case "DFS":
                DFS();
                break;
            case "AStar":
                AStar();
                break;
            case "Dijkstra":
                Dijkstra();
                break;
            default:
                break;
        }

        CreatePath();
        if (!GameManagerScript.alreadyAnimated)
            StartCoroutine(AnimateVisited());
        else ColorVisited();
    }

    private void BFS()
    {
        queue.Enqueue(gameManager.GetStartOrEndNode("Start"));
        while (queue.Count > 0 && isExploring)
        {
            Node searchingNode = queue.Dequeue();
            ExploreNeighbors(searchingNode, "BFS");
        }
    }

    private void DFS()
    {
        stack.Push(gameManager.GetStartOrEndNode("Start"));
        while (stack.Count > 0 && isExploring)
        {
            Node searchingNode = stack.Pop();
            ExploreNeighbors(searchingNode, "DFS");
        }
    }

    private void ExploreNeighbors(Node searchingNode, string algorithmType)
    {
        Node end = gameManager.GetStartOrEndNode("End");
        if (IsInBounds(searchingNode.getX(), searchingNode.getY()) && IsValidNode(searchingNode))
        {
            searchingNode.setVisited(true);
            nodesToAnimate.Add(searchingNode);
        }
        else return;

        if (searchingNode == end)
            isExploring = false;

        List<Node> neighbors = GetNeighbors(searchingNode);

        foreach (Node node in neighbors)
        {
            if (IsValidNode(node))
            {
                if (algorithmType == "BFS")
                    queue.Enqueue(node);
                if (algorithmType == "DFS")
                    stack.Push(node);
                node.setWasExploredFrom(searchingNode);
            }
        }
    }

    private void AStar()
    {
        Node start = gameManager.GetStartOrEndNode("Start");
        Node end = gameManager.GetStartOrEndNode("End");

        List<Node> openSet = new List<Node> { start };

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost() < currentNode.fCost() || openSet[i].fCost() == currentNode.fCost() && openSet[i].getHCost() < currentNode.getHCost())
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            currentNode.setVisited(true);
            nodesToAnimate.Add(currentNode);

            if (currentNode == end)
                return;

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!IsValidNode(neighbor))
                    continue;

                int newNeighborMovementCost = currentNode.getGCost() + GetNodesDistance(currentNode, neighbor);
                if (newNeighborMovementCost < neighbor.getGCost() || !openSet.Contains(neighbor))
                {
                    neighbor.setGCost(newNeighborMovementCost);
                    neighbor.setHCost(GetNodesDistance(neighbor, end));
                    neighbor.setWasExploredFrom(currentNode);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        int GetNodesDistance(Node current, Node next)
        {
            int dx = Mathf.Abs(current.getX() - next.getX());
            int dy = Mathf.Abs(current.getY() - next.getY());
            return dx + dy;
        }
    }

    private void Dijkstra()
    {
        Node start = gameManager.GetStartOrEndNode("Start");
        Node end = gameManager.GetStartOrEndNode("End");
        Dictionary<Node, Node> nextNodeToEnd = new Dictionary<Node, Node>();
        Dictionary<Node, int> costToReachNode = new Dictionary<Node, int>();
        PriorityQueue<Node> queue = new PriorityQueue<Node>();

        queue.Enqueue(start, 0);
        costToReachNode[start] = 0;


        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();
            currentNode.setVisited(true);
            nodesToAnimate.Add(currentNode);

            if (currentNode == end)
                return;

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (IsValidNode(neighbor))
                {
                    int newCost = costToReachNode[currentNode];
                    if (!costToReachNode.ContainsKey(neighbor) || newCost < costToReachNode[neighbor])
                    {
                        costToReachNode[neighbor] = newCost;
                        queue.Enqueue(neighbor, newCost);
                        nextNodeToEnd[neighbor] = currentNode;
                        neighbor.setWasExploredFrom(currentNode);
                    }
                }
            }
        }
    }

    private void CreatePath()
    {
        List<Node> path = new List<Node>();
        Node endNode = GameManagerScript.nodes[(int)gameManager.end.transform.position.x, (int)gameManager.end.transform.position.y];
        path.Add(endNode);
        Node previousNode = endNode.GetWasExploredFrom();
        if (previousNode == null)
        {
            Debug.Log("Path could not be found from the start to the finish!");
            gameManager.GetStartOrEndNode("End").resetColor();
            return;
        }

        while (previousNode.getBox().transform.position != gameManager.start.transform.position)
        {
            path.Add(previousNode);
            previousNode = previousNode.GetWasExploredFrom();
        }

        path.Add(gameManager.GetStartOrEndNode("Start"));
        path.Reverse();

        pathNodesToAnimate = path;
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        int[] dCol = { -1, 0, 1, 0 };
        int[] dRow = { 0, 1, 0, -1 };

        for (int i = 0; i < 4; i++)
        {
            int adjx = node.getX() + dCol[i];
            int adjy = node.getY() + dRow[i];

            if (IsInBounds(adjx, adjy))
                neighbors.Add(GameManagerScript.nodes[adjx, adjy]);
        }

        return neighbors;
    }

    private bool IsInBounds(int x, int y)
    {
        // If cell lies out of bounds
        if (x < 0 || y < 0 ||
            x >= GameManagerScript.arenaWidth || y >= GameManagerScript.arenaHeight)
            return false;

        // Otherwise
        return true;
    }

    bool IsValidNode(Node node)
    {
        // If cell is already visited or is a wall
        if (node.wasVisited() || !node.isActive())
            return false;

        // Otherwise
        return true;
    }

    IEnumerator AnimateVisited()
    {
        GameManagerScript.animating = true;
        foreach (Node node in nodesToAnimate)
        {
            node.getBox().GetComponent<SpriteRenderer>().color = Color.yellow;
            yield return new WaitForSeconds(0.01f);
        }

        foreach (Node node in pathNodesToAnimate)
        {
            node.getBox().GetComponent<SpriteRenderer>().color = Color.cyan;
            yield return new WaitForSeconds(0.02f);
        }
        GameManagerScript.animating = false;
    }

    void ColorVisited()
    {
        foreach (Node node in nodesToAnimate)
            node.getBox().GetComponent<SpriteRenderer>().color = Color.yellow;
        foreach (Node node in pathNodesToAnimate)
            node.getBox().GetComponent<SpriteRenderer>().color = Color.cyan;
    }
}