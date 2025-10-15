using UnityEngine;

public class Foot : MonoBehaviour
{
    private Vector3 lockedPosition;
    private Vector3 initialLocalPosition;
    private bool isLerping;
    private float lerpSpeed;
    private float lerpAmount;
    private float randomDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float timeToLerp;
    [SerializeField] private AnimationCurve liftHeightOverTime;
    [SerializeField] private bool debug;
    private void Awake() {
        lockedPosition = transform.position;
        initialLocalPosition = transform.localPosition;
        lerpSpeed = 1f / timeToLerp;
        lerpAmount = 0f;
        randomDistance = Random.Range(0f, 0.1f);
    }
    private void Update() {
        transform.position = lockedPosition;
        float distance = Vector3.Distance(lockedPosition, transform.parent.TransformPoint(initialLocalPosition));
        if (debug) Debug.Log(distance);
        if (distance >= maxDistance + randomDistance) {
            isLerping = true;
        }
        if (isLerping) {
            lerpAmount += lerpSpeed * Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPosition, lerpAmount);
            //transform.position += Vector3.up * liftHeightOverTime.Evaluate(lerpAmount);
            if (lerpAmount >= 1) {
                lockedPosition = transform.position;
                lerpAmount = 0f;
                isLerping = false;
            }
        }
    }
}
