using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float typewriterSpeed = 0.05f;
    
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    void Start()
    {
        HideDialogue();
    }

    public void ShowDialogue(string text)
    {
        dialoguePanel.SetActive(true);
        
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
            
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }
        
        isTyping = false;
    }

    public bool IsTyping()
    {
        return isTyping;
    }
}