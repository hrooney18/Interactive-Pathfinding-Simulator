using UnityEngine;
using TMPro;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject nodeObjectHolder;

    public GameObject start;
    public GameObject end;

    public static int arenaWidth = 39;
    public static int arenaHeight = 19;
    public static Node[,] nodes = new Node[arenaWidth, arenaHeight];

    public static string currentAlgorithm = "BFS";

    public static bool animating = false;
    public static bool alreadyAnimated = false;

    public static bool tutorialOpen = true;
    [SerializeField] private TextMeshProUGUI runAlgorithmText;

    // Start is called before the first frame update
    void Start()
    {
        SetupNodes();
        runAlgorithmText.text = "Run BFS";
    }

    public void SetCurrentAlgorithm(int algorithmType)
    {
        switch (algorithmType)
        {
            case 0:
                currentAlgorithm = "BFS";
                runAlgorithmText.text = "Run BFS";
                break;
            case 1:
                currentAlgorithm = "DFS";
                runAlgorithmText.text = "Run DFS";
                break;
            case 2:
                currentAlgorithm = "AStar";
                runAlgorithmText.text = "Run A*";
                break;
            case 3:
                currentAlgorithm = "Dijkstra";
                runAlgorithmText.text = "Run Dijkstra's";
                break;
            case 4:
                currentAlgorithm = "Maze";
                runAlgorithmText.text = "Make Maze";
                break;
            default: break;
        }
    }

    public void FindAnimatedPath()
    {
        if (!tutorialOpen && !animating)
        {
            alreadyAnimated = false;
            FindPath(currentAlgorithm);
            alreadyAnimated = true;
        }
    }

    public void FindPath(string algorithm)
    {
        if (!tutorialOpen && !animating)
        {
            ResetNodes();
            gameObject.GetComponent<PathfindingScript>().ResetAndRunAlgorithm(algorithm);
        }
    }

    private void ResetNodes()
    {
        foreach (Node node in nodes)
        {
            node.setVisited(false);
            node.setWasExploredFrom(null);
            if (node.isActive())
                node.resetColor();
        }
    }

    private void SetupNodes()
    {
        for (int y = 0; y < arenaHeight; y++)
        {
            for (int x = 0; x < arenaWidth; x++)
            {
                GameObject box = Instantiate(boxPrefab, new Vector3(x, y, 0), Quaternion.identity);
                box.transform.parent = nodeObjectHolder.transform;
                var isOffset = (x + y) % 2 == 1;
                nodes[x, y] = new Node(x, y, box, true, false, box.GetComponent<InitBox>().InitColor(isOffset));
            }
        }
    }

    public bool IsStartOrEndNode(Node node)
    {
        if (node.getBox().transform.position != start.transform.position && node.getBox().transform.position != end.transform.position)
            return false;
        return true;
    }

    public Node GetStartOrEndNode(string node)
    {
        if (node == "Start")
            return nodes[(int)start.transform.position.x, (int)start.transform.position.y];
        else return nodes[(int)end.transform.position.x, (int)end.transform.position.y];
    }

    public Node GetNode(int x, int y)
    {
        return nodes[x, y];
    }

    public void ClearTrees()
    {
        foreach (Node node in nodes)
        {
            if (node != GetStartOrEndNode("Start") && node != GetStartOrEndNode("End"))
            {
                node.setActive(true);
                node.getBox().GetComponent<NodeController>().ClearTree();
            }
        }
        if (alreadyAnimated)
            FindPath(currentAlgorithm);
    }
}