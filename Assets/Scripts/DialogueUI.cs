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
    private string currentFullText = "";
    
    void Start()
    {
        HideDialogue();
    }
    
    public void ShowDialogue(string text)
    {
        dialoguePanel.SetActive(true);
        
        // Stop any existing typing
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        
        // Clear the text immediately before typing new line
        dialogueText.text = "";
        currentFullText = text;
        
        typingCoroutine = StartCoroutine(TypeText(text));
    }
    
    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        currentFullText = "";
        
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
    
    // Complete the current text instantly
    public void CompleteText()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        
        dialogueText.text = currentFullText;
        isTyping = false;
    }
    
    public bool IsTyping()
    {
        return isTyping;
    }
}