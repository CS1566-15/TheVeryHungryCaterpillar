using UnityEngine;

public class SlugController : MonoBehaviour
{
    [Header("Animation")]
    private Animator animator;
    
    [Header("Dialogue Settings")]
    public DialogueData dialogueData;
    public DialogueUI dialogueUI;
    public InteractPrompt interactPrompt;
    
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionRange = 2f;
    
    private Transform player;
    private int currentDialogueIndex = 0;
    private bool playerInRange = false;
    private bool isInDialogue = false;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        
        if (dialogueUI == null)
            dialogueUI = FindFirstObjectByType<DialogueUI>();
            
        if (interactPrompt == null)
            interactPrompt = FindFirstObjectByType<InteractPrompt>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("Player found: " + player.name);
        }
        else
        {
            Debug.LogError("No GameObject with 'Player' tag found!");
        }
        
        if (dialogueUI == null)
            Debug.LogError("DialogueUI not found or assigned!");
        if (interactPrompt == null)
            Debug.LogError("InteractPrompt not found or assigned!");
        if (dialogueData == null)
            Debug.LogWarning("DialogueData not assigned!");
    }
    
    void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            playerInRange = distance <= interactionRange;
            
            if (playerInRange && !isInDialogue)
            {
                Debug.Log("Player in range! Distance: " + distance);
                if (interactPrompt != null)
                {
                    interactPrompt.ShowPrompt();
                    Debug.Log("Showing interact prompt");
                }
            }
            else
            {
                if (interactPrompt != null)
                    interactPrompt.HidePrompt();
            }
            
            if (playerInRange && Input.GetKeyDown(interactKey))
            {
                Debug.Log("Interact key pressed!");
                if (!isInDialogue)
                {
                    StartDialogue();
                }
                else
                {
                    AdvanceDialogue();
                }
            }
        }
    }
    
    void StartDialogue()
    {
        if (dialogueData == null || dialogueData.dialogueLines.Length == 0)
        {
            Debug.LogWarning("No dialogue data or dialogue lines are empty!");
            return;
        }
        
        Debug.Log("Starting dialogue");
        isInDialogue = true;
        currentDialogueIndex = 0;
        
        if (animator != null)
            animator.SetTrigger("Talk");
        
        ShowCurrentDialogue();
    }
    
    void AdvanceDialogue()
    {
        if (dialogueUI.IsTyping())
        {
            StopAllCoroutines();
            dialogueUI.ShowDialogue(dialogueData.dialogueLines[currentDialogueIndex]);
            return;
        }
        
        currentDialogueIndex++;
        
        if (currentDialogueIndex < dialogueData.dialogueLines.Length)
        {
            ShowCurrentDialogue();
        }
        else
        {
            EndDialogue();
        }
    }
    
    void ShowCurrentDialogue()
    {
        string dialogueToShow = dialogueData.dialogueLines[currentDialogueIndex];
        dialogueUI.ShowDialogue(dialogueToShow);
        interactPrompt.ShowPrompt("Press E to continue");
    }
    
    void EndDialogue()
    {
        dialogueUI.HideDialogue();
        isInDialogue = false;
        currentDialogueIndex = 0;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}