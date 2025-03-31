using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class StickerHandler : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject[] modulePanels;
    public Button[] moduleButtons;
    private int activeModule = -1;
    private Color defaultColor = new Color(0.2902f, 0.5647f, 0.8863f); // #616A6A
    private Color selectedColor = new Color(1.0f, 1.0f, 1.0f); // #CEF4FA
    private Color stickerAppliedColor = new Color(0.9f, 0.7f, 0.2f, 0);
    private HashSet<int> stickerAppliedButtons = new HashSet<int>();

    void Start()
    {
        ResetButtonColors();
        APIManager.Instance.GetRequest("/patient/modules", OnModulesReceived);
    }

    void OnModulesReceived(APIResponse response)
    {
        if (response.Success)
        {
            ModuleSticker[] moduleStickers = JsonHelper.FromJson<ModuleSticker>(response.Data);
            foreach (var moduleSticker in moduleStickers)
            {
                int buttonIndex = GetButtonIndex(moduleSticker.moduleID);
                if (buttonIndex >= 0 && buttonIndex < moduleButtons.Length)
                {
                    ChangeImage(buttonIndex, moduleSticker.stickerID);
                    stickerAppliedButtons.Add(buttonIndex);
                }
            }
        }
    }

    int GetButtonIndex(int moduleId)
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            switch (moduleId)
            {
                case 4: return 0;
                case 5: return 1;
                case 6: return 2;
                case 7: return 3;
                case 8: return 4;
                case 9: return 5;
                default: return -1;
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            switch (moduleId)
            {
                case 10: return 0;
                case 11: return 1;
                case 7: return 2;
                case 8: return 3;
                case 9: return 4;
                default: return -1;
            }
        }
        else
        {
            return -1;
        }
    }

    public void ChangeImage(int buttonIndex, int stickerId)
    {
        if (moduleButtons != null && stickerId >= 1 && stickerId <= 8)
        {
            string spriteName = "Stickers/Sticker" + stickerId;
            Sprite loadedSprite = Resources.Load<Sprite>(spriteName);

            if (loadedSprite != null)
            {
                Debug.Log("Loaded sprite: " + spriteName);
                moduleButtons[buttonIndex].image.sprite = loadedSprite;
                moduleButtons[buttonIndex].image.color = Color.white;

                TextMeshProUGUI buttonText = moduleButtons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.color = stickerAppliedColor;
                }
            }
            else
            {
                Debug.LogError("Sprite not found: " + spriteName);
            }
        }
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

            if (stickerAppliedButtons.Contains(level))
            {
                moduleButtons[level].GetComponentInChildren<TextMeshProUGUI>().color = stickerAppliedColor;
            }
            else
            {
                moduleButtons[level].GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
            }

            activeModule = level;
        }
    }

    private void ResetButtonColors()
    {
        foreach (Button btn in moduleButtons)
        {
            TextMeshProUGUI buttonText = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (stickerAppliedButtons.Contains(System.Array.IndexOf(moduleButtons, btn)))
                {
                    buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 0f);
                }
                else
                {
                    buttonText.color = defaultColor;
                }
            }
        }
    }
}

[System.Serializable]
public class ModuleSticker
{
    public int moduleID;
    public int stickerID;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"array\": " + json + " }";
        Debug.Log("Formatted JSON for Unity: " + newJson);

        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
