using UnityEngine;

public class CaterpillarControl : MonoBehaviour
{
    private CharacterController characterController;
    private bool isMoving;
    private float timeElapsedSinceMoveBegan;
    private float extraTurnSpeedFactor;

    [SerializeField] private float speed;
    [SerializeField] private float extraTurnSpeed;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private float timeSpentMoving;
    [SerializeField] private float delayAfterMoving;
    [SerializeField] private float rotationSmoothing;
    [SerializeField] private float moveTargetSmoothing;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform moveTarget;
    [SerializeField] private Transform caterpillarFront;
    private void Awake() {
        characterController = GetComponent<CharacterController>();
    }
    private void Update() {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
            moveTarget.position = Vector3.Lerp(moveTarget.position, hit.point, Time.deltaTime * moveTargetSmoothing);
        }
        if (Input.GetMouseButtonDown(0)) {
            isMoving = true;
        }
        if (isMoving) {
            // This is a weird workaround, don't worry about it...
            Debug.Log(caterpillarFront.localEulerAngles.x);
            if (caterpillarFront.localEulerAngles.x > 300) {
                extraTurnSpeedFactor = extraTurnSpeed * (360 % caterpillarFront.localEulerAngles.x / 15f);
            }
            else {
                extraTurnSpeedFactor = extraTurnSpeed * (caterpillarFront.localEulerAngles.x / 15f);
            }

            animator.Play("Crawl");
            timeElapsedSinceMoveBegan += Time.deltaTime;
            if (timeElapsedSinceMoveBegan < timeSpentMoving) {
                characterController.Move(caterpillarFront.up * (speed + extraTurnSpeedFactor) * speedCurve.Evaluate(timeElapsedSinceMoveBegan) * Time.deltaTime);
                transform.forward = -Vector3.Lerp(-transform.forward, caterpillarFront.up, Time.deltaTime * rotationSmoothing);
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
            }
            if (timeElapsedSinceMoveBegan >= delayAfterMoving + timeSpentMoving) {
                timeElapsedSinceMoveBegan = 0f;
                if (!Input.GetMouseButton(0)) {
                    isMoving = false;
                }
            }
        }
    }
}
