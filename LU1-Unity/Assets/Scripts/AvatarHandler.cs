using System.Globalization;
using System.Xml.Linq;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class AvatarHandler : MonoBehaviour
{

    public Button routeAButton;
    public Button routeBButton;
    public Button startButton;

    private Color routeDefaultColor = new Color(0.722f, 1.0f, 1.0f);
    private Color routeSelectedColor = new Color(1.0f, 1.0f, 1.0f);
    private enum Route { None, RouteA, RouteB }
    private Route selectedRoute = Route.None;

    public Button[] avatarButtons;
    public Image[] colorPanels;

    private Color defaultColor = new Color(162f / 255f, 228f / 255f, 184f / 255f); 
    private Color selectedColor = new Color(184f / 255f, 255f / 255f, 255f / 255f); 
    public int _avatarIndex = 0;

    public TextMeshProUGUI Result;
    public TMP_InputField Name, geboortedatum, arts, afspraak;

    void Start()
    {
        ResetAvatarColors();
        APIManager.Instance.GetRequest("/patient/me", OnModuleReceived);

        ResetButtonColors();

        routeAButton.onClick.AddListener(SelectRouteA);
        routeBButton.onClick.AddListener(SelectRouteB);
        startButton.onClick.AddListener(onStartPressed);
        for (int i = 0; i < avatarButtons.Length; i++)
        {
            int index = i; 
            avatarButtons[i].onClick.AddListener(() => SelectAvatar(index));
        }
    }

   
    public void SelectAvatar(int avatarIndex)
    {
        _avatarIndex = avatarIndex;
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

    private void OnModuleReceived(APIResponse response)
    {
        if (response.Success)
            SceneManager.LoadScene(2);
    }

    public void onDateChanged()
    {
        if (!IsValidDate(geboortedatum.text))
        {
            Result.text = "Geboortedatum moet in formaat dd-mm-jjjj";
            return;
        }
        Result.text = "";
    }
    public void onAfspraakChanged()
    {
        if (!IsValidDate(afspraak.text))
        {
            Result.text = "Afspraakdatum moet in formaat dd-mm-jjjj";
            return;
        }
        Result.text = "";
    }
    private bool IsValidDate(string date)
    {
        return DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }
    private string ConvertToISO8601(string date)
    {
        if (DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        {
            return parsedDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
        return null;
    }


    // Start het behandelplan
    public void onStartPressed()
    {
        string behandelplan = "B";
        switch (selectedRoute)
        {
            case Route.RouteA:
                behandelplan = "A";
                break;

            case Route.RouteB:
                behandelplan = "B";
                break;
        }
        if (selectedRoute == Route.None)
        {
            Debug.Log("Kies eerst een route!");
            return;
        }

        if (!IsValidDate(geboortedatum.text) || !IsValidDate(afspraak.text))
        {
            Result.text = "Controleer de datuminvoer.";
            return;
        }

        Patient patient = new Patient
        {
            naam = Name.text,
            geboortedatum = ConvertToISO8601(geboortedatum.text),
            behandelplan = behandelplan,
            arts = arts.text,
            eersteAfspraak = ConvertToISO8601(afspraak.text),
            avatarID = _avatarIndex
        };

        string json = JsonUtility.ToJson(patient);
        Debug.Log("Sending patient data: " + json);

        APIManager.Instance.PostRequest("/patient/create", json, OnPatientReceived);
    }
    private void OnPatientReceived(APIResponse response)
    {
        Debug.Log("Patient creation response: " + response.Data);

        if (response.Success)
        {
            switch (selectedRoute)
            {
                case Route.RouteA:
                    SceneManager.LoadScene("RouteA");
                    break;

                case Route.RouteB:
                    SceneManager.LoadScene("RouteB");
                    break;
            }
        }
        else
        {
            Result.text = "Er is iets fout gegaan, probeer het opnieuw";
        }
    }

    // Route selectie
    public void SelectRouteA()
    {
        routeAButton.GetComponentInChildren<TextMeshProUGUI>().color = routeSelectedColor;
        routeBButton.GetComponentInChildren<TextMeshProUGUI>().color = routeDefaultColor;

        selectedRoute = Route.RouteA;
    }

    public void SelectRouteB()
    {

        routeAButton.GetComponentInChildren<TextMeshProUGUI>().color = routeDefaultColor;
        routeBButton.GetComponentInChildren<TextMeshProUGUI>().color = routeSelectedColor;

        selectedRoute = Route.RouteB;
    }

    private void ResetButtonColors()
    {
        routeAButton.GetComponentInChildren<TextMeshProUGUI>().color = routeDefaultColor;
        routeBButton.GetComponentInChildren<TextMeshProUGUI>().color = routeDefaultColor;
    }
}
