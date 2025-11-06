using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;

public class CaterpillarControl : MonoBehaviour
{
    // References
    private AudioSource audioSource;
    // Movement
    private bool canMove;
    private bool isMoving;
    private bool moveTargetIsActive;
    private float timeElapsedSinceMoveBegan;
    private float speedMultiplier = 1f;
    // Gravity and Jumping
    private bool isWaitingToJump;
    private float yVelocity;
    private bool hasLanded;
    private float jumpDistance;
    private float lastMagnitude;
    private Vector3 jumpDirection;
    private float timeElapsedSinceLanding;
    private int numberOfJumpsRemaining;
    private float increasedJumpDistance;
    // Eating (growing size)
    private bool isCurrentlyGrowing;
    private float timeSpentGrowing;
    private float targetCameraSizeAfterGrowth;
    private float sizeIncreaseAfterEating;
    private float growthMultiplier = 1f;
    private float currentSize = 1f;
    // Segment Interval (see comments on 'ProcessSegmentsOnInterval')
    private string intervalType;
    private float interval;
    private bool segmentIntervalEnabled;
    private int currentSegment;
    private float nextIntervalTime;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float extraTurnSpeed;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private float timeSpentMoving;
    [SerializeField] private float delayAfterMoving;
    [SerializeField] private float rotationSmoothing;
    [SerializeField] private float moveTargetSmoothing;
    [SerializeField] private Transform[] bodySegments;
    [Header("Gravity and Jumping")]
    [SerializeField] private float gravity;
    [SerializeField] private float verticalJumpSpeed;
    [SerializeField] private float horizontalJumpSpeedMultiplier;
    [SerializeField] private JumpPathGenerator jumpPathGenerator;
    [SerializeField] private float timeToMoveAfterLanding;
    [Header("Sounds")]
    [SerializeField] private AudioClip jumpStart;
    [SerializeField] private AudioClip jumpClick;
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform moveTarget;
    [SerializeField] private Transform caterpillarFront;
    [SerializeField] private AudioClip sizeGrowthSound;
    [SerializeField] private Transform jumpCounterParent;

    private void Awake() {
        yVelocity = -20f;
        canMove = true;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        ProcessMoveTarget();
        ProcessMovement();
        ProcessGravityAndJumping();
        ProcessGrowingSize();
        ProcessLimitingMaxYVelocityForSegments();
        ProcessSegmentsOnInterval();
    }

    private void ProcessMoveTarget() {
        if (!moveTargetIsActive) return;
        // The caterpillar always moves toward the moveTarget. This is used for moving and jumping.
        // Use the cursor position to fire a raycast, which hits the ground. This is the point the caterpillar moves toward.
        if (!isWaitingToJump) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, 1 << 8)) {
                moveTarget.position = Vector3.Lerp(moveTarget.position, new Vector3(hit.point.x, transform.position.y, hit.point.z), Time.deltaTime * moveTargetSmoothing);
            }
        }
        else {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
                moveTarget.position = Vector3.Lerp(moveTarget.position, hit.point, Time.deltaTime * moveTargetSmoothing * 10);
                Vector3 vec = moveTarget.position - characterController.transform.position;
                if (vec.magnitude >= 10f + increasedJumpDistance) {
                    vec = vec.normalized * (10f + increasedJumpDistance);
                    moveTarget.position = characterController.transform.position + vec;
                }
                if (Mathf.Abs(vec.magnitude - lastMagnitude) >= 1f) {
                    lastMagnitude = vec.magnitude;
                    audioSource.pitch = vec.magnitude / 10f;
                    audioSource.PlayOneShot(jumpClick);
                }
            }
        }
    }

    private void ProcessMovement() {
        if (!canMove) return;
        // Start moving by clicking mouse button
        if (Input.GetMouseButtonDown(0)) {
            isMoving = true;
        }
        else if (Input.GetMouseButtonUp(0)) {
            isMoving = false;
        }
        if (isMoving) {
            Vector3 movementDirection = (moveTarget.position - characterController.transform.position).normalized;
            characterController.Move(movementDirection * speed * speedMultiplier * Time.deltaTime);
            characterController.transform.forward = new Vector3(-movementDirection.x, 0, -movementDirection.z);
        }
    }

    private void ProcessGravityAndJumping() {
        // Get ready to jump when user presses RMB.
        if (Input.GetMouseButtonDown(1)) {
            if (!characterController.isGrounded) return;
            if (numberOfJumpsRemaining <= 0) {
                jumpCounterParent.GetComponent<Animation>().Play("JumpCounterEmpty");
                return;
            }
            jumpPathGenerator.ShowPath();
            audioSource.PlayOneShot(jumpStart);
            moveTarget.GetComponent<Animation>().Play("MoveTargetShow");
            isWaitingToJump = true;
            canMove = false;
        }
        // Start the jump when user releases RMB.
        // There's A LOT of stuff to do when the player jumps.
        // "The dumping ground"
        else if (Input.GetMouseButtonUp(1)) {
            if (!isWaitingToJump) return;
            jumpPathGenerator.HidePath();
            isWaitingToJump = false;
            moveTarget.GetComponent<Animation>().Play("MoveTargetHide");
            audioSource.pitch = 1f;
            float extraVelocity = moveTarget.position.y - transform.position.y;
            yVelocity = verticalJumpSpeed + extraVelocity;
            moveTargetIsActive = false;
            jumpDistance = Vector3.Distance(characterController.transform.position, moveTarget.position);
            jumpDirection = -caterpillarFront.forward;
            gravity = -40f;
            Physics.gravity = new Vector3(0f, -40f, 0f);
            for (int i = 1; i < bodySegments.Length; i++) {
                bodySegments[i].GetComponent<Rigidbody>().useGravity = false;
            }
            numberOfJumpsRemaining -= 1;
            jumpCounterParent.GetChild(0).GetComponent<TMP_Text>().SetText(numberOfJumpsRemaining.ToString());
        }
        // Runs while RMB is held down. Rotate the caterpillar toward the moveTarget.
        if (isWaitingToJump) {
            // Makes a flat vector on the plane Y=0 from caterpillar to moveTarget.
            Vector3 lookDirection = new Vector3(moveTarget.position.x - characterController.transform.position.x, 0, moveTarget.position.z - characterController.transform.position.z);
            characterController.transform.forward = -lookDirection;
        }
        characterController.Move(Vector3.up * yVelocity * Time.deltaTime);
        // Add gravity and move the caterpillar through the air.
        if (!characterController.isGrounded) {
            if (hasLanded) {
                hasLanded = false;
            }
            yVelocity += gravity * Time.deltaTime;
            characterController.Move(jumpDirection * horizontalJumpSpeedMultiplier * jumpDistance * Time.deltaTime);
            //transform.GetChild(0).forward = Vector3.Lerp(transform.GetChild(0).forward, new Vector3(-characterController.velocity.x, -yVelocity * 0.5f, -characterController.velocity.z), Time.deltaTime * 30f);
        }
        // Check when we've landed...play sound and animation.
        else {
            //transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.identity, Time.deltaTime * 80f);
            // Again, there's A LOT of stuff to do when we detect that the player has landed.
            if (!hasLanded) {
                for (int i = 1; i < bodySegments.Length; i++) {
                    bodySegments[i].GetComponent<Rigidbody>().useGravity = true;
                }
                gravity = -2f;
                SegmentIntervalLand();
                characterController.Move(jumpDirection * speed * Time.deltaTime);
                timeElapsedSinceLanding += Time.deltaTime;
                if (timeElapsedSinceLanding >= timeToMoveAfterLanding) {
                    Physics.gravity = new Vector3(0f, -2f, 0f);
                    timeElapsedSinceLanding = 0;
                    canMove = true;
                    moveTargetIsActive = true;
                    hasLanded = true;
                }
            }
        }
    }

    public void StartToGrowCaterpillarSize(float sizeIncreaseAfterEating) {
        currentSize += sizeIncreaseAfterEating;
        this.sizeIncreaseAfterEating = sizeIncreaseAfterEating * growthMultiplier;
        targetCameraSizeAfterGrowth = Camera.main.orthographicSize * sizeIncreaseAfterEating;
        audioSource.PlayOneShot(sizeGrowthSound);
        SegmentIntervalSwallow();
        isCurrentlyGrowing = true;
    }

    private void ProcessGrowingSize() {
        if (!isCurrentlyGrowing) return;
        // Smoothing lerp between old caterpillar size and camera size.
        timeSpentGrowing += Time.deltaTime;
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetCameraSizeAfterGrowth, timeSpentGrowing);
        if (timeSpentGrowing >= 1f) {
            //SegmentIntervalSizeGrowth();
            speed *= sizeIncreaseAfterEating;
            extraTurnSpeed *= sizeIncreaseAfterEating;
            timeSpentGrowing = 0f;
            isCurrentlyGrowing = false;
        }
    }

    private void ProcessLimitingMaxYVelocityForSegments() {
        for (int i = 1; i < bodySegments.Length; i++) {
            Vector3 vel = bodySegments[i].GetComponent<Rigidbody>().linearVelocity;
            if (vel.y < -10f) {
                bodySegments[i].GetComponent<Rigidbody>().linearVelocity = new Vector3(vel.x, -10f, vel.y);
            }
        }
    }

    private void SegmentIntervalRigidbodyActivation() {
        intervalType = "rigidbody";
        interval = 0.05f;
        segmentIntervalEnabled = true;
    }

    private void SegmentIntervalSwallow() {
        intervalType = "swallow";
        interval = 0.05f;
        segmentIntervalEnabled = true;
    }

    private void SegmentIntervalSizeGrowth() {
        intervalType = "size growth";
        interval = 0.0f;
        segmentIntervalEnabled = true;
    }

    private void SegmentIntervalLand() {
        intervalType = "land";
        interval = 0.0f;
        segmentIntervalEnabled = true;
    }

    // This allows us to apply an effect to each body segment gradually, over time.
    // Because it looks nicer to go segment-by-segment rather than
    // applying the effect to every segment at the same time.
    // Currently it supports...
    // - Rigidbody activation, which happens when the caterpillar lands after a jump.
    // - Size growth, which happens when caterpillar eats food.
    private void ProcessSegmentsOnInterval() {
        if (!segmentIntervalEnabled) return;
        // ------------------------------------------------------------------
        // If the operation happnes EVERY FRAME during interval, put it here.
        // ------------------------------------------------------------------

        if (Time.time >= nextIntervalTime) {
            // -------------------------------------------
            // If the operation happnes ONCE, put it here.
            // -------------------------------------------
            if (intervalType == "rigidbody") {
                if (currentSegment != 0) {
                    bodySegments[currentSegment].GetComponent<Rigidbody>().useGravity = true;
                }
            }
            if (intervalType == "swallow") {
                bodySegments[currentSegment].GetComponent<BodySegment>().Swallow(sizeIncreaseAfterEating);
            }
            if (intervalType == "land") {
                bodySegments[currentSegment].GetComponent<BodySegment>().Land();
            }
            // Process moving on to the next segment.
            currentSegment += 1;
            if (currentSegment >= bodySegments.Length) {
                currentSegment = 0;
                segmentIntervalEnabled = false;
                return;
            }
            nextIntervalTime = Time.time + interval;
        }
    }

    public void IncreaseNumberOfJumps() {
        numberOfJumpsRemaining += 1;
        // Actual text component is child of jumpCounterParent
        jumpCounterParent.GetChild(0).GetComponent<TMP_Text>().SetText(numberOfJumpsRemaining.ToString());
        jumpCounterParent.GetComponent<Animation>().Play("JumpCounterIncrease");
    }

    public void SetHasIncreasedJumpDistance(bool hasIncreasedJumpDistance) {
        if (hasIncreasedJumpDistance) {
            increasedJumpDistance = 5f;
        }
        else {
            increasedJumpDistance = 0f;
        }
    }

    public void SetSpeedMultiplier(bool hasSpeedMultiplier) {
        if (hasSpeedMultiplier) {
            speedMultiplier = 2f;
        }
        else {
            speedMultiplier = 1f;
        }
    }

    public void SetGrowthMultiplier(bool hasGrowthMultiplier) {
        if (hasGrowthMultiplier) {
            growthMultiplier = 2f;
        }
        else {
            growthMultiplier = 1f;
        }
    }

    public bool GetIsWaitingToJump()
    {
        return isWaitingToJump;
    }
    
    public float GetCurrentSize()
    {
        return currentSize;
    }
}
