using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class CaterpillarControl : MonoBehaviour
{
    // References
    private CharacterController characterController;
    private AudioSource audioSource;
    // Movement
    private bool canMove;
    private bool isMoving;
    private bool moveTargetIsActive;
    private float timeElapsedSinceMoveBegan;
    private float extraTurnSpeedFactor;
    // Gravity and Jumping
    private bool isWaitingToJump;
    private float yVelocity;
    private bool hasLanded;
    private float jumpDistance;
    private Vector3 jumpDirection;
    // Eating (growing size)
    private bool isCurrentlyGrowing;
    private float timeSpentGrowing;
    private float targetCameraSizeAfterGrowth;
    private float sizeIncreaseAfterEating;
    private Vector3 targetSize;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float extraTurnSpeed;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private float timeSpentMoving;
    [SerializeField] private float delayAfterMoving;
    [SerializeField] private float rotationSmoothing;
    [SerializeField] private float moveTargetSmoothing;
    [Header("Gravity and Jumping")]
    [SerializeField] private float gravity;
    [SerializeField] private float verticalJumpSpeed;
    [SerializeField] private float horizontalJumpSpeedMultiplier;
    [SerializeField] private JumpPathGenerator jumpPathGenerator;
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform moveTarget;
    [SerializeField] private Transform caterpillarFront;
    [SerializeField] private AudioClip sizeGrowthSound;

    private void Awake() {
        canMove = true;
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        ProcessMoveTarget();
        ProcessMovement();
        ProcessGravityAndJumping();
        ProcessGrowingSize();
    }

    private void ProcessMoveTarget() {
        if (!moveTargetIsActive) return;
        // The caterpillar always moves toward the moveTarget. This is used for moving and jumping.
        // Use the cursor position to fire a raycast, which hits the ground. This is the point the caterpillar moves toward.
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
            // Smooth out the moveTarget's position so the caterpillar looks less robotic when aiming toward the moveTarget.
            if (!isWaitingToJump) {
                moveTarget.position = Vector3.Lerp(moveTarget.position, hit.point, Time.deltaTime * moveTargetSmoothing);
            }
            else {
                moveTarget.position = Vector3.Lerp(moveTarget.position, hit.point, Time.deltaTime * moveTargetSmoothing * 10);
            }
        }
    }

    private void ProcessMovement() {
        if (!canMove) return;
        // Start moving by clicking mouse button
        if (Input.GetMouseButtonDown(0)) {
            isMoving = true;
        }
        if (isMoving) {
            // This 'if, else' block adds speed when the caterpillar is turning...for some reason, the movement doesn't look right otherwise.
            // Kind of a bandaid solution, but it works.

            if (caterpillarFront.localEulerAngles.x > 300) {
                extraTurnSpeedFactor = Mathf.Abs(extraTurnSpeed * (360 % caterpillarFront.localEulerAngles.x / 15f));
            }
            else {
                extraTurnSpeedFactor = Mathf.Abs(extraTurnSpeed * (caterpillarFront.localEulerAngles.x / 15f));
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

    private void ProcessGravityAndJumping() {
        // Get ready to jump when user presses RMB.
        if (Input.GetMouseButtonDown(1)) {
            jumpPathGenerator.ShowPath();
            animator.Play("Prejump");
            moveTarget.GetComponent<Animation>().Play("MoveTargetShow");
            isWaitingToJump = true;
            canMove = false;
        }
        // Start the jump when user releases RMB.
        else if (Input.GetMouseButtonUp(1)) {
            jumpPathGenerator.HidePath();
            isWaitingToJump = false;
            moveTarget.GetComponent<Animation>().Play("MoveTargetHide");
            yVelocity = verticalJumpSpeed;
            moveTargetIsActive = false;
            jumpDistance = Vector3.Distance(transform.position, moveTarget.position);
            jumpDirection = caterpillarFront.up;
        }
        // Runs while RMB is held down. Rotate the caterpillar toward the moveTarget.
        if (isWaitingToJump) {
            // Makes a flat vector on the plane Y=0 from caterpillar to moveTarget.
            Vector3 lookDirection = new Vector3(moveTarget.position.x - transform.position.x, 0, moveTarget.position.z - transform.position.z);
            transform.forward = -lookDirection;
        }
        // Add gravity and move the caterpillar through the air.
        if (!characterController.isGrounded) {
            if (hasLanded) {
                hasLanded = false;
            }
            yVelocity += gravity * Time.deltaTime;
            characterController.Move(jumpDirection * horizontalJumpSpeedMultiplier * jumpDistance * Time.deltaTime);
            transform.GetChild(0).forward = characterController.velocity;
        }
        // Check when we've landed...play sound and animation.
        else {
            if (!hasLanded) {

                canMove = true;
                moveTargetIsActive = true;
                hasLanded = true;
            }
        }
        characterController.Move(Vector3.up * yVelocity * Time.deltaTime);
    }

    public void StartToGrowCaterpillarSize(float sizeIncreaseAfterEating) {
        this.sizeIncreaseAfterEating = sizeIncreaseAfterEating;
        targetSize = transform.localScale * sizeIncreaseAfterEating;
        targetCameraSizeAfterGrowth = Camera.main.orthographicSize * sizeIncreaseAfterEating;
        audioSource.PlayOneShot(sizeGrowthSound);
        isCurrentlyGrowing = true;
    }

    private void ProcessGrowingSize() {
        if (!isCurrentlyGrowing) return;
        // Smoothing lerp between old caterpillar size and camera size.
        timeSpentGrowing += Time.deltaTime;
        transform.localScale = Vector3.Lerp(transform.localScale, targetSize, timeSpentGrowing);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetCameraSizeAfterGrowth, timeSpentGrowing);
        if (timeSpentGrowing >= 1f) {
            speed *= sizeIncreaseAfterEating;
            extraTurnSpeed *= sizeIncreaseAfterEating;
            timeSpentGrowing = 0f;
            isCurrentlyGrowing = false;
        }
    }

    public bool GetIsWaitingToJump() {
        return isWaitingToJump;
    }
}
