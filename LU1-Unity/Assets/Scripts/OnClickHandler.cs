using System;
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


    public void MarkDone(int module)
    {
        int randomSticker = UnityEngine.Random.Range(1, 5);
        string url = $"/patient/mark-module-done?moduleId={module}&stickerId={randomSticker}";
        APIManager.Instance.PostRequest(url, "", OnMarkDoneResponse);
    }

    void OnMarkDoneResponse(APIResponse response)
    {
        if (response.Success)
        {
            Debug.Log("Module marked as done");
            APIManager.Instance.GetRequest("/patient/me", OnModuleReceived);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    private void OnModuleReceived(APIResponse response)
    {
        if (response.Success && response.Data != null)
        {
            try
            {
                Patient responseData = JsonUtility.FromJson<Patient>(response.Data.ToString());

                if (responseData != null)
                {
                    switch (responseData.behandelplan)
                    {
                        case "A":
                            SceneManager.LoadScene("RouteA");
                            break;
                        case "B":
                            SceneManager.LoadScene("RouteB");
                            break;
                        default:
                            Debug.LogError("Onbekend behandelplan: " + responseData.behandelplan);
                            break;
                    }
                }
                else
                {
                    SceneManager.LoadScene(1);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("JSON Parsing Error: " + ex.Message);
            }
        }
    }
}
