using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialCanvas;

    [SerializeField] private GameObject[] tutorialPanels;

    public void ShowHideTutorial(bool show)
    {
        if (show)
        {
            tutorialCanvas.SetActive(true);
            GameManagerScript.tutorialOpen = true;
        }
        else
        {
            tutorialCanvas.SetActive(false);
            GameManagerScript.tutorialOpen = false;
        }
    }

    public void ShowTutorialPanel(int tutorialIndex)
    {
        for (int i = 0; i < tutorialPanels.Length; i++)
        {
            if (i == tutorialIndex)
                tutorialPanels[i].SetActive(true);
            else tutorialPanels[i].SetActive(false);

        }
    }
}
