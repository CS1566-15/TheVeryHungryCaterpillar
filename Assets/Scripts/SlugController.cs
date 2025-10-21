using UnityEngine;

public class SlugController : MonoBehaviour
{
    private Animator animator;
    private float timer = 0f;
    private float talkInterval = 10f;
   
    void Start()
    {
        animator = GetComponent<Animator>();
    }
   
    void Update()
    {
        // Increment timer
        timer += Time.deltaTime;
        
        // Trigger talk animation every 10 seconds
        if (timer >= talkInterval)
        {
            animator.SetTrigger("Talk");
            timer = 0f;
        }
    }
}