using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI dialogueText;
    public GameObject blurOverlay;
    private string[] lines;
    private int index;

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
        if (blurOverlay != null)
            blurOverlay.SetActive(false);
    }

    public void StartDialogue(string[] newLines)
    {
        lines = newLines;
        index = 0;
        panel.SetActive(true);
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
        panel.SetActive(false);
        blurOverlay.SetActive(false);
        //GameManager.Instance.OnDialogueFinished();
    }
}
