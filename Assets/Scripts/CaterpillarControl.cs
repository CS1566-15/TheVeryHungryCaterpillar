using UnityEngine;
using UnityEngine.Audio;

public class CaterpillarControl : MonoBehaviour
{
    private CharacterController characterController;
    private AudioSource audioSource;
    private bool isMoving;
    private float timeElapsedSinceMoveBegan;
    private float extraTurnSpeedFactor;
    private bool isCurrentlyGrowingSize;
    private Vector3 targetSize;
    private float timeSpentGrowingSize;
    private float cameraTargetOrthographicSizeAfterGrowth;

    [Header("Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float extraTurnSpeed;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private float timeSpentMoving;
    [SerializeField] private float delayAfterMoving;
    //                          Higher value = less smooth.
    [SerializeField] private float rotationSmoothing;
    [SerializeField] private float moveTargetSmoothing;
    [SerializeField] private float sizeIncreaseAfterEating;
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform moveTarget;
    [SerializeField] private Transform caterpillarFront;
    [SerializeField] private AudioClip sizeGrowthSound;
    private void Awake() {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Update() {
        ProcessMovement();
        ProcessGrowingSize();
    }
    private void ProcessMovement() {
        // Use the cursor position to fire a raycast, which hits the ground. This is the point the caterpillar moves toward.
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
            // Smooth out the moveTarget's position so the caterpillar looks less robotic when aiming toward the moveTarget.
            moveTarget.position = Vector3.Lerp(moveTarget.position, hit.point, Time.deltaTime * moveTargetSmoothing);
        }
        // Start moving by clicking mouse button
        if (Input.GetMouseButtonDown(0)) {
            isMoving = true;
        }
        if (isMoving) {
            // This 'if, else' block adds speed when the caterpillar is turning...for some reason, the movement doesn't look right otherwise.
            // Kind of a bandaid solution, but it works.
            if (caterpillarFront.localEulerAngles.x > 300) {
                extraTurnSpeedFactor = extraTurnSpeed * (360 % caterpillarFront.localEulerAngles.x / 15f);
            }
            else {
                extraTurnSpeedFactor = extraTurnSpeed * (caterpillarFront.localEulerAngles.x / 15f);
            }
            // A singular Caterpillar movement is split into 2 parts:
            // 1. actually moving
            // 2. delay after moving, so the animation can catch up
            animator.Play("Crawl");
            // This keeps track of time, so we know when to stop moving.
            timeElapsedSinceMoveBegan += Time.deltaTime;
            // This block only executes while the caterpillar is allowed to move.
            if (timeElapsedSinceMoveBegan < timeSpentMoving) {
                // Moves the caterpillar in the direction the caterpillarFront is facing (its head) and rotates the entire body.
                characterController.Move(caterpillarFront.up * (speed + extraTurnSpeedFactor) * speedCurve.Evaluate(timeElapsedSinceMoveBegan) * Time.deltaTime);
                transform.forward = -Vector3.Lerp(-transform.forward, caterpillarFront.up, Time.deltaTime * rotationSmoothing);
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
            }
            // This checks if 1. and 2. have completed (total time elapsed is greater than the delay + time spent moving).
            // If so, that means we've completed a single movement, and we set isMoving to false.
            if (timeElapsedSinceMoveBegan >= delayAfterMoving + timeSpentMoving) {
                timeElapsedSinceMoveBegan = 0f;
                if (!Input.GetMouseButton(0)) {
                    isMoving = false;
                }
            }
        }
    }
    public void StartToGrowCaterpillarSize() {
        targetSize = transform.localScale * sizeIncreaseAfterEating;
        cameraTargetOrthographicSizeAfterGrowth = Camera.main.orthographicSize * sizeIncreaseAfterEating;
        audioSource.PlayOneShot(sizeGrowthSound);
        isCurrentlyGrowingSize = true;
    }

    private void ProcessGrowingSize() {
        if (!isCurrentlyGrowingSize) return;
        timeSpentGrowingSize += Time.deltaTime;
        transform.localScale = Vector3.Lerp(transform.localScale, targetSize, timeSpentGrowingSize);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, cameraTargetOrthographicSizeAfterGrowth, timeSpentGrowingSize);
        if (timeSpentGrowingSize >= 1f) {
            speed *= sizeIncreaseAfterEating;
            extraTurnSpeed *= sizeIncreaseAfterEating;
            timeSpentGrowingSize = 0f;
            isCurrentlyGrowingSize = false;
        }
    }
}
