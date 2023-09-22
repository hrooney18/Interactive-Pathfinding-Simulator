using UnityEngine;

public class InitBox : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private Color baseColor, offsetColor;

    public Color InitColor(bool isOffset)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Color color = isOffset ? offsetColor : baseColor;
        renderer.color = color;
        return color;
    }

    private void OnMouseEnter()
    {
        highlight.SetActive(true);
    }
    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }
}
