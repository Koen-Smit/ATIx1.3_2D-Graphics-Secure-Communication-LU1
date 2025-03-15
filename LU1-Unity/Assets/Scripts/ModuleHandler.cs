using UnityEngine;

public class ModuleHandler : MonoBehaviour
{
    public TMPro.TextMeshProUGUI moduleTitle;
    void Start()
    {
        moduleTitle.text = "Module ";
    }

    public void LoadModule(int level)
    {
        moduleTitle.text = "Module " + level;
    }

}