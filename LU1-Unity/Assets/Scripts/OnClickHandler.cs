using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickHandler : MonoBehaviour
{
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
            APIManager.Instance.GetRequest("/account/username", OnUsernameReceived);
    }
    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void LogOut()
    {
        APIManager.Instance.PostRequest("/account/logout", "", OnLogoutResponse);
    }

    void OnUsernameReceived(APIResponse response)
    {
        if (!response.Success || response.StatusCode == 405)
        { 
            Debug.Log("Logging out...");
            APIManager.Instance.PostRequest("/account/logout", "", OnLogoutResponse);
        }
        else
            Debug.Log("Logged in...");
    }

    void OnLogoutResponse(APIResponse response)
    {
        if (response.Success)
        {
            PlayerPrefs.DeleteKey("authToken");
            APIManager.Instance.SetAuthToken("");
            SceneManager.LoadScene(0);
        }
    }
}
