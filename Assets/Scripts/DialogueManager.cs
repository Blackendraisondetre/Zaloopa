using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

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
        public float percent = 50.0f;
    }

    public static DialogueManager Instance;
    
    [Header("Popup UI")]
    public TextMeshProUGUI actionPopupText;
    public CanvasGroup actionPopupCanvasGroup;
    public float popupDuration = 2f;

    [Header("UI Panels")]
    public GameObject dialoguePanel;
    public GameObject actionCardPanel;
    public GameObject blurOverlay;

    [Header("Text & UI")]
    public TextMeshProUGUI dialogueText;
    public HouseUI houseui;

    [Header("House Data")]
    public List<HouseData> houseList = new List<HouseData>();

    [Header("Game State")]
    public int day = 0;
    public int counterbrainwash = 0;
    public int counterkill = 0;

    private string[] lines;
    private int index;
    private int id_house;

    void Awake() {
        if (Instance == null) Instance = this;
    }

    void Start() {
        index = 0;
        dialoguePanel?.SetActive(false);
        blurOverlay?.SetActive(false);
        actionCardPanel?.SetActive(false);
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
            dialoguePanel?.SetActive(true);
            blurOverlay?.SetActive(true);
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
        if (dialogueText != null && index < lines.Length) {
            dialogueText.text = lines[index];
        }
    }

    void EndDialogue() {
        dialoguePanel?.SetActive(false);
        updateid(id_house);
        actionCardPanel?.SetActive(true);
    }

    void updateid(int _id) {
        id_house = _id;
        Debug.Log("id " + id_house);
    }

    public void Kill() {
        Debug.Log("Killing house with id: " + id_house);

        foreach (HouseData hd in houseList) {
            if (hd.idofhouse == id_house) {
                Debug.Log("Found GameObject: " + hd.go.name + " with percent: " + hd.percent);

                HouseUI ui = hd.go.GetComponent<HouseUI>();
                if (ui != null) {
                    ui.SetState(HouseState.Dead);
                    ui.UpdateVisual();
                } else {
                    Debug.LogWarning("No HouseUI component found on: " + hd.go.name);
                }

                if (day == 2) {
                    // Future: update day panel here
                }

                endActionCard();
                DisableHouseButton();
                ShowPopupMessage("House has been killed!");
                return;
            }
        }

        Debug.LogWarning("No matching house found for id: " + id_house);
    }

    public void Brainwash()
{
    Debug.Log("Attempting to brainwash house with id: " + id_house);

    foreach (HouseData hd in houseList)
    {
        if (hd.idofhouse == id_house)
        {
            float chance = hd.percent;
            float roll = Random.Range(0f, 100f);
            Debug.Log($"Brainwash chance: {chance}% | Roll: {roll}");

            if (roll <= chance)
            {
                Debug.Log("Brainwash successful!");

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

                counterbrainwash++;
                DisableHouseButton();
                ShowPopupMessage("House was brainwashed!");
            }
            else
            {
                Debug.Log("Brainwash failed. House remains unchanged.");
                ShowPopupMessage("Brainwashing failed.");
            }

            endActionCard();
            return;
        }
    }

    Debug.LogWarning("No matching house GameObject found for id: " + id_house);
}


    public void endActionCard() {
        actionCardPanel?.SetActive(false);
        blurOverlay?.SetActive(false);
    }

    public void DisableHouseButton() {
        foreach (HouseData hd in houseList) {
            if (hd.idofhouse == id_house) {
                Button button = hd.go.GetComponent<Button>();
                if (button != null) {
                    button.enabled = false;  // Completely disables the component
                    Debug.Log("Disabled Button component on: " + hd.go.name);
                } else {
                    Debug.LogWarning("No Button component found on: " + hd.go.name);
                }
                return;
            }
        }

        Debug.LogWarning("No house found to disable button for id: " + id_house);
    }

    public void ShowPopupMessage(string message)
{
    StopAllCoroutines(); // Cancel any running popup coroutine
    StartCoroutine(ShowPopupRoutine(message));
}

private IEnumerator ShowPopupRoutine(string message)
{
    actionPopupText.text = message;
    actionPopupCanvasGroup.alpha = 1f;
    actionPopupText.gameObject.SetActive(true);

    yield return new WaitForSeconds(popupDuration);

    // Fade out
    float fadeDuration = 0.5f;
    float t = 0f;
    while (t < fadeDuration)
    {
        t += Time.deltaTime;
        actionPopupCanvasGroup.alpha = 1f - (t / fadeDuration);
        yield return null;
    }

    actionPopupCanvasGroup.alpha = 0f;
    actionPopupText.gameObject.SetActive(false);
}

}
