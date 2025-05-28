using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

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

    [Header("Brainwash Progress UI")]
    public Slider brainwashSlider;
    public TextMeshProUGUI brainwashPercentText;

    [Header("Day Transition UI")]
    public GameObject dayPanel;
    public TextMeshProUGUI dayText;
    public float dayDisplayTime = 2f;

    [Header("Persistent UI")]
    public TextMeshProUGUI currentDayText;
    public TextMeshProUGUI actionsLeftText;

    [Header("House Data")]
    public List<HouseData> houseList = new List<HouseData>();

    [Header("Action Buttons")]
    public Button killButton;

    [Header("Game State")]
    public int dayCounter = 0;
    public int counterkill = 0;

    private string[] lines;
    private int index;
    private int id_house;

    void Awake() {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        index = 0;
        dialoguePanel?.SetActive(false);
        blurOverlay?.SetActive(false);
        actionCardPanel?.SetActive(false);
        dayPanel?.SetActive(false);
        UpdateBrainwashProgress();
        UpdateCurrentDayUI();
        UpdateActionsLeftUI();
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
                    UpdateBrainwashProgress();
                } else {
                    Debug.LogWarning("No HouseUI component found on: " + hd.go.name);
                }

                counterkill++;

                 if (counterkill >= 2 && killButton != null)
                {
                killButton.interactable = false;
                Debug.Log("Kill button disabled — kill limit reached.");
                }

                endActionCard();
                DisableHouseButton();
                ShowPopupMessage("House has been killed!");
                dayCounter++;
                CheckDayProgress();
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
                    UpdateBrainwashProgress();
                }
                else
                {
                    Debug.LogWarning("No HouseUI component found on: " + hd.go.name);
                }
                DisableHouseButton();
                ShowPopupMessage("House was brainwashed!");
            }
            else
            {
                Debug.Log("Brainwash failed. House remains unchanged.");
                ShowPopupMessage("Brainwashing failed.");
            }
            dayCounter++;
            endActionCard();
            CheckDayProgress();
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

private void CheckDayProgress()
{
    UpdateCurrentDayUI();
    UpdateActionsLeftUI();
    // Check for day transitions
        if (dayCounter == 2 || dayCounter == 4)
        {
            int currentDay = (dayCounter / 2) + 1;
            StartCoroutine(ShowDayTransition(currentDay));
        }

    // End game after 6 actions or all houses are done
    if (dayCounter >= 6 || AllHousesHandled())
    {
        EndGame();
    }
}

private IEnumerator ShowDayTransition(int dayNumber)
{
    UpdateCurrentDayUI();
    dayText.text = $"Day {dayNumber}";
    dayPanel.SetActive(true);
    blurOverlay?.SetActive(true);

    yield return new WaitForSeconds(dayDisplayTime);

    dayPanel.SetActive(false);
    blurOverlay?.SetActive(false);
}

private bool AllHousesHandled()
{
    foreach (var hd in houseList)
    {
        HouseUI ui = hd.go.GetComponent<HouseUI>();
        if (ui != null && ui.state == HouseState.Neutral)
        {
            return false; // At least one house still in play
        }
    }
    return true;
}

    private void UpdateBrainwashProgress()
    {
        int totalRelevant = 0;
        int brainwashed = 0;

        foreach (var hd in houseList)
        {
            HouseUI ui = hd.go.GetComponent<HouseUI>();
            if (ui != null && ui.state != HouseState.Dead)
            {
                totalRelevant++;
                if (ui.state == HouseState.Brainwashed)
                {
                    brainwashed++;
                }
            }
        }

        float percent = totalRelevant > 0 ? (brainwashed / (float)totalRelevant) * 100f : 0f;

        if (brainwashSlider != null)
            brainwashSlider.value = percent;

        if (brainwashPercentText != null)
            brainwashPercentText.text = $"Voters: {Mathf.RoundToInt(percent)}%";
    }

    private void UpdateCurrentDayUI()
    {
        int currentDay = (dayCounter / 2) + 1;
        if (currentDayText != null)
        {
            currentDayText.text = $"Day {currentDay}";
        }
    }

    private void UpdateActionsLeftUI()
    {
        int actionsThisDay = dayCounter % 2;
        int actionsLeft = 2 - actionsThisDay;

        if (actionsLeftText != null)
        {
            actionsLeftText.text = $"Actions Left: {actionsLeft}";
        }
    }


    private void EndGame()
    {
        int totalAlive = 0;
        int brainwashedCount = 0;

        foreach (var hd in houseList)
        {
            HouseUI ui = hd.go.GetComponent<HouseUI>();
            if (ui != null)
            {
                if (ui.state != HouseState.Dead)
                {
                    totalAlive++;
                    if (ui.state == HouseState.Brainwashed)
                    {
                        brainwashedCount++;
                    }
                }
            }
        }

        Debug.Log($"Remaining non-dead houses: {totalAlive}, Brainwashed: {brainwashedCount}");

        if (brainwashedCount > totalAlive / 2f)
        {
            SceneManager.LoadScene("WinScreen");
        }
        else
        {
            SceneManager.LoadScene("LoseScreen");
        }
    }





}
