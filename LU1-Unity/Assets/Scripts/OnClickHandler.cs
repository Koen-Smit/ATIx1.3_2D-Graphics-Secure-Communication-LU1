using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickHandler : MonoBehaviour
{
    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
}
