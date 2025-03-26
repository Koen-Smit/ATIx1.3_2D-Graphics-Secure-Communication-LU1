using UnityEngine;
using UnityEngine.UI;

public class AvatarHandler : MonoBehaviour
{
    public Button[] avatarButtons;

    public Image[] colorPanels;

    private Color defaultColor = new Color(162f / 255f, 228f / 255f, 184f / 255f); // Groen

    // Kleur voor de geselecteerde avatar
    private Color selectedColor = new Color(184f / 255f, 255f / 255f, 255f / 255f); // Wit (de geselecteerde kleur)

    void Start()
    {
        // Zorg ervoor dat alle vakjes de groene kleur hebben bij het starten van het spel
        ResetAvatarColors();

        // Voeg de methodes toe aan de knoppen zodat ze de SelectAvatar methode aanroepen
        for (int i = 0; i < avatarButtons.Length; i++)
        {
            int index = i;  // Local variable for lambda expression to capture the correct index
            avatarButtons[i].onClick.AddListener(() => SelectAvatar(index));
        }
    }

    // Methode om de kleur te veranderen wanneer een avatar wordt geselecteerd
    public void SelectAvatar(int avatarIndex)
    {
        // Verander de kleur van het gekleurde vakje van de geselecteerde avatar
        for (int i = 0; i < colorPanels.Length; i++)
        {
            if (i == avatarIndex)
                colorPanels[i].color = selectedColor;  // Stel de geselecteerde kleur in (bijvoorbeeld wit)
            else
                colorPanels[i].color = defaultColor;  // Zet de andere vakjes terug naar groen
        }
    }

    // Reset alle kleuren naar de groene kleur (beginkleur)
    private void ResetAvatarColors()
    {
        for (int i = 0; i < colorPanels.Length; i++)
        {
            colorPanels[i].color = defaultColor;  // Zet alle vakjes op groen bij het starten
        }
    }
}
