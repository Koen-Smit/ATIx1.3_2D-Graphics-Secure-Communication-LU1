using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModuleHandler : MonoBehaviour
{
    public TMPro.TextMeshProUGUI moduleTitle;
    public GameObject startPanel;
    public GameObject[] modulePanels;
    public Button[] moduleButtons;
    private int activeModule = -1;
    private Color defaultColor = new Color(0.2902f, 0.5647f, 0.8863f); // #616A6A
    private Color selectedColor = new Color(1.0f, 1.0f, 1.0f); // #CEF4FA

    void Start()
    {
        ResetButtonColors();
    }

    public void LoadModule(int level)
    {
        if (activeModule == level)
        {
            modulePanels[level].SetActive(false);
            startPanel.SetActive(true);
            ResetButtonColors();
            activeModule = -1;
        }
        else
        {
            foreach (GameObject panel in modulePanels)
            {
                panel.SetActive(false);
            }

            startPanel.SetActive(false);
            modulePanels[level].SetActive(true);
            ResetButtonColors();
            moduleButtons[level].GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
            activeModule = level;
        }
    }

    private void ResetButtonColors()
    {
        foreach (Button btn in moduleButtons)
        {
            btn.GetComponentInChildren<TextMeshProUGUI>().color = defaultColor;
        }
    }
}