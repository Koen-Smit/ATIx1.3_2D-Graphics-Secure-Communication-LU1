using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject panel;

    public void OpenMenu()
    {
        panel.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        panel.gameObject.SetActive(false);
    }
}
