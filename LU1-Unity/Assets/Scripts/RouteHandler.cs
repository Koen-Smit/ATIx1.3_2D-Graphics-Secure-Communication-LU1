using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class RouteHandler : MonoBehaviour
{
    public Button routeAButton;
    public Button routeBButton;
    public Button startButton;

    private Color defaultColor = new Color(0.722f, 1.0f, 1.0f);
    private Color selectedColor = new Color(1.0f, 1.0f, 1.0f);  

    // Met de enum wordt er bijgehouden welke route is geselecteerd
    private enum Route { None, RouteA, RouteB }
    private Route selectedRoute = Route.None;

    void Start()
    {
        ResetButtonColors();

        routeAButton.onClick.AddListener(SelectRouteA);
        routeBButton.onClick.AddListener(SelectRouteB);
        startButton.onClick.AddListener(StartRoute);
    }

    // Method om route A te selecteren en de kleur te veranderen
    public void SelectRouteA()
    {
        routeAButton.GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
        routeBButton.GetComponentInChildren<TextMeshProUGUI>().color = defaultColor;

        // De gekozen route wordt opgeslagen als RouteA in de enum
        selectedRoute = Route.RouteA;
    }

    // Method om route B te selecteren en de kleur te veranderen
    public void SelectRouteB()
    {
        
        routeAButton.GetComponentInChildren<TextMeshProUGUI>().color = defaultColor;
        routeBButton.GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;

        // De gekozen route wordt opgeslagen als RouteB in de enum
        selectedRoute = Route.RouteB;
    }

    private void ResetButtonColors()
    {
        routeAButton.GetComponentInChildren<TextMeshProUGUI>().color = defaultColor;
        routeBButton.GetComponentInChildren<TextMeshProUGUI>().color = defaultColor;
    }

    // Methode die wordt aangeroepen wanneer de startknop wordt ingedrukt
    public void StartRoute()
    {
        if (selectedRoute == Route.None)
        {
            Debug.Log("Kies eerst een route!");
            return;
        }

     

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
}
