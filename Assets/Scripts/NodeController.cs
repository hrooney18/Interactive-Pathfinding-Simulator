using UnityEngine;
using UnityEngine.EventSystems;

public class NodeController : MonoBehaviour
{
    GameManagerScript gameManager;

    private bool draggingWalls = false;
    private bool draggingStartOrEnd = false;
    private string draggedPoint = "";
    [SerializeField] private GameObject tree;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            draggingWalls = true;
        if (Input.GetMouseButtonUp(0))
        {
            draggingWalls = false;
            draggingStartOrEnd = false;
        }
    }

    private void OnMouseEnter()
    {
        if (!GameManagerScript.tutorialOpen && !GameManagerScript.animating && !EventSystem.current.IsPointerOverGameObject())
        {
            if (draggingWalls && !draggingStartOrEnd)
            {
                Node currentNode = GetThisNode();

                currentNode.resetColor();
                if (!gameManager.IsStartOrEndNode(currentNode))
                    currentNode.setActive(!currentNode.isActive());
                if (!currentNode.isActive())
                    tree.SetActive(true);
                if (currentNode.isActive())
                    tree.SetActive(false);

                if (GameManagerScript.alreadyAnimated)
                    FindObjectOfType<GameManagerScript>().FindPath(GameManagerScript.currentAlgorithm);
            }
        }
    }

    private void OnMouseDown()
    {
        if (!GameManagerScript.tutorialOpen && !GameManagerScript.animating && !EventSystem.current.IsPointerOverGameObject())
        {
            if (gameObject.transform.position == gameManager.start.transform.position || gameObject.transform.position == gameManager.end.transform.position)
            {
                draggingStartOrEnd = true;
                draggedPoint = StartOrEndDragged();
            }
            else
            {
                Node currentNode = GetThisNode();

                currentNode.resetColor();
                if (!gameManager.IsStartOrEndNode(currentNode))
                    currentNode.setActive(!currentNode.isActive());
                if (!currentNode.isActive())
                    tree.SetActive(true);
                if (currentNode.isActive())
                    tree.SetActive(false);

                if (GameManagerScript.alreadyAnimated)
                    FindObjectOfType<GameManagerScript>().FindPath(GameManagerScript.currentAlgorithm);
            }
        }
    }

    private void OnMouseDrag()
    {
        if (!GameManagerScript.tutorialOpen && !GameManagerScript.animating && !EventSystem.current.IsPointerOverGameObject())
        {
            if (draggingStartOrEnd)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos = new Vector3(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y), 0);

                if (Mathf.RoundToInt(mousePos.x) >= 0 && Mathf.RoundToInt(mousePos.y) >= 0 &&
                    Mathf.RoundToInt(mousePos.x) < GameManagerScript.arenaWidth && Mathf.RoundToInt(mousePos.y) < GameManagerScript.arenaHeight)
                {
                    Node nodeHovered = GameManagerScript.nodes[Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y)];
                    if (nodeHovered != gameManager.GetStartOrEndNode("Start") && nodeHovered != gameManager.GetStartOrEndNode("End"))
                    {
                        nodeHovered.resetColor();
                        nodeHovered.setActive(true);
                        if (draggedPoint == "Start")
                            gameManager.start.transform.position = mousePos;
                        else if (draggedPoint == "End")
                            gameManager.end.transform.position = mousePos;
                    }
                }
            }
        }
    }

    private string StartOrEndDragged()
    {
        if (gameObject.transform.position == gameManager.start.transform.position)
            return "Start";
        return "End";
    }

    public void ClearTree()
    {
        tree.SetActive(false);
    }

    public void SetTree()
    {
        tree.SetActive(true);
    }

    public Node GetThisNode()
    {
        return GameManagerScript.nodes[Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.y)];
    }
}
