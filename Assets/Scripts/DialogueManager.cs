using UnityEngine;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine {
    public int id;
    public string text;
    public string emotion;
}

[System.Serializable]
public class DialogueData {
    public string dialogueId;
    public string npcName;
    public DialogueLine[] lines;
}

public class DialogueManager : MonoBehaviour {
    [System.Serializable]
    public class HouseData {
        public int idofhouse;
        public GameObject go;
        public float percent= 50.0f;
    }

    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public GameObject actionCardPanel;
    public TextMeshProUGUI dialogueText;
    public GameObject blurOverlay;
    public HouseUI houseui;

    public List<HouseData> houseList = new List<HouseData>(); 

    private string[] lines;
    private int index;
    private int id_house;
    public int day = 0;
    public int counterbrainwash = 0;
    public int counterkill = 0;

    void Awake() {
        if (Instance == null) Instance = this;
    }

    
    void Start() {
        index = 0;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (blurOverlay != null)
            blurOverlay.SetActive(false);
        if (actionCardPanel != null)
            actionCardPanel.SetActive(false);
    }

    public void StartDialogueForHouse(int house_id) {
        id_house = house_id;

        string fileName = $"dialogue_00{house_id}";
        TextAsset jsonFile = Resources.Load<TextAsset>("Dialogues/" + fileName);

        if (jsonFile != null) {
            DialogueData dialogue = JsonUtility.FromJson<DialogueData>(jsonFile.text);
            lines = new string[dialogue.lines.Length];

            for (int i = 0; i < dialogue.lines.Length; i++) {
                lines[i] = dialogue.lines[i].text;
                Debug.Log(lines[i]);
            }

            index = 0;
            dialoguePanel.SetActive(true);
            blurOverlay.SetActive(true);
            ShowLine();
        } else {
            Debug.LogError("Dialogue file not found: " + fileName);
        }
    }

    public void NextLine() {
        index++;
        if (index >= lines.Length) {
            EndDialogue();
        } else {
            ShowLine();
        }
    }

    void ShowLine() {
        if (dialogueText != null && index < lines.Length)
            dialogueText.text = lines[index];
    }

    void EndDialogue() {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        updateid(id_house);
                
        if (actionCardPanel != null)
            actionCardPanel.SetActive(true);
    }
    void updateid(int _id)
{
    id_house = _id;
    Debug.Log("id" + id_house);
}
    public void Kill()
{
    Debug.Log("Killing house with id: " + id_house);

    foreach (HouseData hd in houseList)
    {
        if (hd.idofhouse == id_house)
        {
            Debug.Log("Found GameObject for house: " + hd.go.name);
            Debug.Log("Found GameObject for house: " + hd.percent);

            HouseUI ui = hd.go.GetComponent<HouseUI>();
            if (ui != null)
            {
                ui.SetState(HouseState.Dead);
                ui.UpdateVisual();
            }
            else
            {
                Debug.LogWarning("No HouseUI component found on: " + hd.go.name);
            }
            if( day == '2'){
                ////Day panel should be changed //
            }
            return;
        }
    }


    Debug.LogWarning("No matching house GameObject found for id: " + id_house);
}

    public void Brainwash()
{
    Debug.Log("Killing house with id: " + id_house);
        counterbrainwash ++;
        Debug.Log("Counter for the brainwash" + counterbrainwash);
    foreach (HouseData hd in houseList)
    {
        if (hd.idofhouse == id_house)
        {
            Debug.Log("Found GameObject for house: " + hd.go.name);

            HouseUI ui = hd.go.GetComponent<HouseUI>();
            if (ui != null)
            {
                ui.SetState(HouseState.Brainwashed);
                ui.UpdateVisual();
            }
            else
            {
                Debug.LogWarning("No HouseUI component found on: " + hd.go.name);
            }

            return;
        }
    }

    Debug.LogWarning("No matching house GameObject found for id: " + id_house);
}
}


