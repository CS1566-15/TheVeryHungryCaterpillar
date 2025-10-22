using UnityEngine;

public class CameraIdleWobble : MonoBehaviour
{
    [Header("Wobble Settings")]
    public float wobbleSpeed = 0.5f;   // how fast the wobble cycles
    public float wobbleAmount = 2f;    // how many degrees of tilt
    public float wobbleOffset = 1.3f;  // phase offset so X and Y aren’t in sync

    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        float xWobble = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        float yWobble = Mathf.Sin((Time.time * wobbleSpeed) + wobbleOffset) * wobbleAmount;

        transform.localRotation = originalRotation * Quaternion.Euler(xWobble, yWobble, 0f);
    }
}
