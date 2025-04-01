using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class DagboekHandler : MonoBehaviour
{
    public GameObject notitiePanel;
    public GameObject notePrefab;
    public Transform NotitieContentPanel, AfspraakContentPanel;
    public TMP_InputField inputfield;

    //menu related
    public GameObject Keuze_frame, Afspraak_frame, Notitie_frame;
    void Start()
    {
        CloseNotitiePanel();
        OpenKeuze();
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

    //menu related
    public void OpenKeuze()
    {
        Keuze_frame.SetActive(true);
        Afspraak_frame.SetActive(false);
        Notitie_frame.SetActive(false);
    }

    public void OpenAfspraak()
    {
        Keuze_frame.SetActive(false);
        Afspraak_frame.SetActive(true);
        Notitie_frame.SetActive(false);
    }

    public void OpenNotitie()
    {
        Keuze_frame.SetActive(false);
        Afspraak_frame.SetActive(false);
        Notitie_frame.SetActive(true);
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

                Transform targetPanel = note.note.Contains("@afspraak") ? AfspraakContentPanel : NotitieContentPanel;

                GameObject noteObject = Instantiate(notePrefab, targetPanel);
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
