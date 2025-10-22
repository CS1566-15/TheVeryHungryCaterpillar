using UnityEngine;
using TMPro;

public class InteractPrompt : MonoBehaviour
{
    public GameObject promptPanel;
    public TextMeshProUGUI promptText;
    
    void Start()
    {
        HidePrompt();
    }
    
    public void ShowPrompt(string text = "Press E to interact")
    {
        promptPanel.SetActive(true);
        promptText.text = text;
    }
    
    public void HidePrompt()
    {
        promptPanel.SetActive(false);
    }
}