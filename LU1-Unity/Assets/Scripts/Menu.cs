using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject panel;

    public void OpenMenu()
    {
        //opent settings menu
        panel.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        panel.gameObject.SetActive(false);
    }
}


