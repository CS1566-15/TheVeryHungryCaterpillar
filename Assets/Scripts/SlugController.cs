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

    private Transform player;
    private int currentDialogueIndex = 0;
    private bool playerInRange = false;
    private bool isInDialogue = false;
    private bool hasBeenTriggered = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (dialogueUI == null)
            dialogueUI = FindFirstObjectByType<DialogueUI>();

        if (interactPrompt == null)
            interactPrompt = FindFirstObjectByType<InteractPrompt>();

        if (dialogueData == null)
            Debug.LogWarning("DialogueData not assigned!");
    }

    void Update()
    {
        // Show prompt only when player is in range, not in dialogue, and hasn't triggered yet
        if (playerInRange && !isInDialogue && !hasBeenTriggered)
        {
            if (interactPrompt != null)
            {
                interactPrompt.ShowPrompt();
            }
        }
        else
        {
            if (interactPrompt != null)
                interactPrompt.HidePrompt();
        }

        // Handle interaction
        if (playerInRange && Input.GetKeyDown(interactKey) && !hasBeenTriggered)
        {
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;

            // If player leaves during dialogue, end it
            if (isInDialogue)
            {
                EndDialogue();
            }
        }
    }

    void StartDialogue()
    {
        if (dialogueData == null || dialogueData.dialogueLines.Length == 0)
        {
            Debug.LogWarning("No dialogue data!");
            return;
        }

        isInDialogue = true;
        currentDialogueIndex = 0;

        if (animator != null)
            animator.SetTrigger("Talk");

        ShowCurrentDialogue();
    }

    void AdvanceDialogue()
    {
        // If still typing, complete the current line instantly
        if (dialogueUI.IsTyping())
        {
            dialogueUI.CompleteText();
            return;
        }

        // Move to next line
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogueData.dialogueLines.Length)
        {
            // Show next line (ShowDialogue already clears text automatically)
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
    }

    void EndDialogue()
    {
        dialogueUI.HideDialogue();
        isInDialogue = false;
        currentDialogueIndex = 0;
        hasBeenTriggered = true;

        // Hide the prompt permanently
        if (interactPrompt != null)
            interactPrompt.HidePrompt();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = hasBeenTriggered ? Color.gray : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 4.3f);
    }
}