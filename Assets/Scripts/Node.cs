using UnityEngine;

public class Node
{
    int x;
    int y;
    GameObject box;
    bool active;
    bool visited;
    Node wasExploredFrom;

    int gCost;
    int hCost;
    Color startingColor;

    public Node(int x, int y, GameObject box, bool active, bool visited, Color startingColor)
    {
        this.x = x;
        this.y = y;
        this.box = box;
        this.active = active;
        this.visited = visited;
        this.startingColor = startingColor;
    }

    public int getX() { return x; }
    public int getY() { return y; }
    public GameObject getBox() { return box; }
    public bool isActive() { return active; }
    public bool wasVisited() { return visited; }
    public Node GetWasExploredFrom() { return wasExploredFrom; }
    public int getGCost() { return gCost; }
    public int getHCost() { return hCost; }
    public int fCost() { return gCost + hCost; }
    public void setActive(bool active) {  this.active = active; }
    public void setVisited(bool visited) { this.visited = visited; }
    public void setWasExploredFrom(Node node) { this.wasExploredFrom = node;}
    public void setGCost(int cost) { gCost = cost; }
    public void setHCost(int cost) { hCost = cost; }
    public void resetColor() { getBox().GetComponent<SpriteRenderer>().color = startingColor; }
}
