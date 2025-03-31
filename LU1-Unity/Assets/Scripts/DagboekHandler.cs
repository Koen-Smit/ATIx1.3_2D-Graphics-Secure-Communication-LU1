using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class DagboekHandler : MonoBehaviour
{
    public GameObject notitiePanel;
    public GameObject notePrefab;
    public Transform contentPanel;
    public TMP_InputField inputfield;

    void Start()
    {
        CloseNotitiePanel();
        FetchNotes();
    }

    public void OpenNotitiePanel()
    {
        notitiePanel.SetActive(true);
    }

    public void CloseNotitiePanel()
    {
        notitiePanel.SetActive(false);
    }
    public void CreateNote()
    {
        string json = "{\"note\":\"" + inputfield.text + "\"}";
        APIManager.Instance.PostRequest("/dagboek/create", json, OnNoteCreatedReceived);
    }

    private void FetchNotes()
    {
        APIManager.Instance.GetRequest("/dagboek/me", OnNotesReceived);
    }

    private void OnNoteCreatedReceived(APIResponse response)
    {
        if (response.Success)
        {
            SceneManager.LoadScene(12);
        }
    }


    private void OnNotesReceived(APIResponse response)
    {
        if (response.Success && !string.IsNullOrEmpty(response.Data))
        {
            Note[] notes = JsonHelper.FromJson<Note>(response.Data);

            if (notes == null || notes.Length == 0) return;

            foreach (Note note in notes)
            {
                if (note == null) continue;

                GameObject noteObject = Instantiate(notePrefab, contentPanel);
                TMP_Text[] texts = noteObject.GetComponentsInChildren<TMP_Text>();

                foreach (TMP_Text text in texts)
                {
                    if (text.name == "tekst") 
                    {
                        text.text = note.note;
                        Debug.Log("Updated tekst: " + note.note);
                    }
                    else if (text.name == "tijd") 
                    {
                        text.text = FormatTimestamp(note.timestamp);
                        Debug.Log("Updated tijd: " + note.timestamp);
                    }
                }

            }
        }
    }

    private string FormatTimestamp(string timestamp)
    {
        DateTime parsedDate = DateTime.Parse(timestamp);
        return parsedDate.ToString("dd-MM-yyyy HH:mm");
    }
}

[Serializable]
public class Note
{
    public string userId;
    public string note;
    public string timestamp;
}
