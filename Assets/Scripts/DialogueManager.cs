using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject dialoguePanel;
    public GameObject actionCardPanel;
    public TextMeshProUGUI dialogueText;
    public GameObject blurOverlay;
    private string[] lines;
    private int index;

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (blurOverlay != null)
            blurOverlay.SetActive(false);
    }

    public void StartDialogueForHouse(int house_id)
    {
        // Read Text dependent on house_id
        dialoguePanel.SetActive(true);
        blurOverlay.SetActive(true);
        ShowLine();
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
        dialogueText.text = lines[index];
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        blurOverlay.SetActive(false);
        //GameManager.Instance.OnDialogueFinished();
    }
}
