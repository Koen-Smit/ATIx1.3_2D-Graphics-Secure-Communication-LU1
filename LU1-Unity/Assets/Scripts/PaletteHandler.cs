using UnityEngine;

public class PaletteHandler : MonoBehaviour
{
    //panels
    public GameObject palettePanel1, palettePanel2, palettePanel3;
    void Start()
    {
        palettePanel1.SetActive(false);
        palettePanel2.SetActive(false);
        palettePanel3.SetActive(false);
    }

    public void LoadPalette(int level)
    {
        switch (level)
        {
            case 1:
                palettePanel1.SetActive(true);
                palettePanel2.SetActive(false);
                palettePanel3.SetActive(false);
                break;
            case 2:
                palettePanel1.SetActive(false);
                palettePanel2.SetActive(true);
                palettePanel3.SetActive(false);
                break;
            case 3:
                palettePanel1.SetActive(false);
                palettePanel2.SetActive(false);
                palettePanel3.SetActive(true);
                break;
        }
    }

}
