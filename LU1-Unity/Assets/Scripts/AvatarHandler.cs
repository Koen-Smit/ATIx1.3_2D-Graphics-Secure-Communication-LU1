using UnityEngine;
using UnityEngine.UI;

public class AvatarHandler : MonoBehaviour
{
    public Button[] avatarButtons;

    public Image[] colorPanels;

    private Color defaultColor = new Color(162f / 255f, 228f / 255f, 184f / 255f); 

    
    private Color selectedColor = new Color(184f / 255f, 255f / 255f, 255f / 255f); 

    void Start()
    {
        
        ResetAvatarColors();

        
        for (int i = 0; i < avatarButtons.Length; i++)
        {
            int index = i; 
            avatarButtons[i].onClick.AddListener(() => SelectAvatar(index));
        }
    }

    
    public void SelectAvatar(int avatarIndex)
    {
        // Verander de kleur van het gekleurde vakje van de geselecteerde avatar
        for (int i = 0; i < colorPanels.Length; i++)
        {
            if (i == avatarIndex)
                colorPanels[i].color = selectedColor;  
            else
                colorPanels[i].color = defaultColor;  
        }
    }

    
    private void ResetAvatarColors()
    {
        for (int i = 0; i < colorPanels.Length; i++)
        {
            colorPanels[i].color = defaultColor; 
        }
    }
}
