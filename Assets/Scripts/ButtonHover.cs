using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool hovering = false;
    private Vector3 originalScale;
    private Quaternion originalRotation;

    private float timer = 0f;
    private int frame = 0;

    void Start()
    {
        originalScale = transform.localScale;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        if (hovering)
        {
            timer += Time.deltaTime;

            // switch frame every 0.15 seconds 
            if (timer >= 0.15f)
            {
                timer = 0f;
                frame = (frame + 1) % 2; // two-frame loop

                if (frame == 0)
                {
                    transform.localScale = originalScale;
                    transform.localRotation = originalRotation;
                }
                else
                {
                    transform.localScale = originalScale * 1.1f;
                    transform.localRotation = Quaternion.Euler(0, 0, 5);
                }
            }
        }
        else
        {
            // Reset when not hovering
            transform.localScale = originalScale;
            transform.localRotation = originalRotation;
            timer = 0f;
            frame = 0;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => hovering = true;
    public void OnPointerExit(PointerEventData eventData) => hovering = false;
}
