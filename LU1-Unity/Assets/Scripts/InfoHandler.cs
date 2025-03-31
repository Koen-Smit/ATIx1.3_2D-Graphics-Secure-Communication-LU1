using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class InfoHandler : MonoBehaviour
{
    public TextMeshProUGUI naamField;
    public TextMeshProUGUI geboortedatumField;
    public TextMeshProUGUI artsField;
    public TextMeshProUGUI eersteAfspraakField;

    private string naam;
    private string geboortedatum;
    private string behandelplan;
    private string arts;
    private string eersteAfspraak;
    private int avatarID;

    public Image uiImage;

    void Start()
    {
        APIManager.Instance.GetRequest("/patient/me", OnInfoReceived);
    }

    void OnInfoReceived(APIResponse response)
    {
        if (response.Success && !string.IsNullOrEmpty(response.Data))
        {
            try
            {
                Patient responseData = JsonUtility.FromJson<Patient>(response.Data);

                if (responseData != null)
                {
                    naam = responseData.naam;
                    geboortedatum = FormatDate(responseData.geboortedatum);
                    behandelplan = responseData.behandelplan;
                    arts = responseData.arts;
                    eersteAfspraak = FormatDate(responseData.eersteAfspraak);
                    avatarID = responseData.avatarID + 1;
                    ChangeImage();

                    if (naamField) naamField.text = naam;
                    if (geboortedatumField) geboortedatumField.text = geboortedatum;
                    if (artsField) artsField.text = arts;
                    if (eersteAfspraakField) eersteAfspraakField.text = eersteAfspraak;
                }
                else
                {
                    Debug.LogError("Failed to deserialize patient data.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Deserialization Error: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("API Response failed or returned empty data.");
            SceneManager.LoadScene(1);
        }
    }

    string FormatDate(string dateString)
    {
        if (DateTime.TryParse(dateString, out DateTime parsedDate))
        {
            return parsedDate.ToString("dd-MM-yyyy");
        }
        return "Ongeldige datum";
    }

    public void ChangeImage()
    {
        if (uiImage != null && avatarID >= 1 && avatarID <= 8)
        {
            string spriteName = "Selection" + avatarID;
            Sprite loadedSprite = Resources.Load<Sprite>(spriteName);

            if (loadedSprite != null)
            {
                uiImage.sprite = loadedSprite;
            }
            else
            {
                Debug.LogError("Sprite not found: " + spriteName);
            }
        }
    }
}
