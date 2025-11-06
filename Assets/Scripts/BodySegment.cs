using UnityEngine;

public class BodySegment : MonoBehaviour
{
    private Vector3 targetSize;
    private Vector3 initialSize;
    private bool isCurrentlySwallowing;
    private bool isCurrentlyLanding;
    private float timeSpent;
    [SerializeField] private AnimationCurve swallowCurve;
    public void Swallow(float sizeIncreaseAfterEating) {
        if (isCurrentlySwallowing) return;
        targetSize = transform.localScale * sizeIncreaseAfterEating;
        isCurrentlySwallowing = true;
    }
    public void Land() {
        if (isCurrentlyLanding) return;
        initialSize = transform.localScale;
        isCurrentlyLanding = true;
    }
    void Update() {
        Swallowing();
        Landing();
    }
    private void Swallowing() {
        if (!isCurrentlySwallowing) return;
        timeSpent += Time.deltaTime * 2f;
        transform.localScale = Vector3.Lerp(transform.localScale, targetSize + Vector3.one * swallowCurve.Evaluate(timeSpent) * 0.5f, timeSpent);
        if (timeSpent >= 1f) {
            timeSpent = 0f;
            isCurrentlySwallowing = false;
        }
    }
    private void Landing() {
        if (!isCurrentlyLanding) return;
        timeSpent += Time.deltaTime * 2;
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(initialSize.x, initialSize.y * (1 - swallowCurve.Evaluate(timeSpent) * 0.5f), initialSize.z), timeSpent);
        if (timeSpent >= 1f) {
            timeSpent = 0f;
            isCurrentlyLanding = false;
        }
    }
}
