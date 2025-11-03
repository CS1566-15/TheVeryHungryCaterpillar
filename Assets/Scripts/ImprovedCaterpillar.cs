using UnityEngine;

public class ImprovedCaterpillar : MonoBehaviour
{
    private Vector3[] pathPointsPositions = new Vector3[3];
    private Quaternion[] pathPointsRotations = new Quaternion[3];
    [SerializeField] private Transform frontTarget;
    [SerializeField] private Transform[] targets;
    [SerializeField] private float distanceBetweenPathPoints;
    private Vector3 movement;
    private void Awake() {
        pathPointsPositions[0] = targets[0].position;
        pathPointsPositions[1] = targets[1].position;
        pathPointsPositions[2] = targets[2].position;
    }
    private void Update() {
        ProcessMovement();

        float distanceFromFrontPathPoint = Vector3.Distance(frontTarget.position, pathPointsPositions[0]);
        float progress = distanceFromFrontPathPoint / distanceBetweenPathPoints;

        pathPointsPositions[2] = Vector3.Lerp(pathPointsPositions[2], pathPointsPositions[1], progress);
        pathPointsPositions[1] = Vector3.Lerp(pathPointsPositions[1], pathPointsPositions[0], progress);
        pathPointsPositions[0] = Vector3.Lerp(pathPointsPositions[0], frontTarget.position, progress);

        if (distanceFromFrontPathPoint >= distanceBetweenPathPoints) {
            pathPointsPositions[2] = pathPointsPositions[1];
            pathPointsPositions[1] = pathPointsPositions[0];
            pathPointsPositions[0] = frontTarget.position;

            pathPointsRotations[2] = pathPointsRotations[1];
            pathPointsRotations[1] = pathPointsRotations[0];
            pathPointsRotations[0] = frontTarget.rotation;
        }
        for (int i = 0; i < 3; i++) {
            targets[i].position = Vector3.Lerp(targets[i].position, pathPointsPositions[i], Time.deltaTime * 5f);
            targets[i].rotation = Quaternion.Lerp(targets[i].rotation, pathPointsRotations[i], Time.deltaTime * 5f);
        }
    }
    private void ProcessMovement() {
        movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) {
            movement.z = 1;
        }
        if (Input.GetKey(KeyCode.A)) {
            movement.x = -1;
        }
        if (Input.GetKey(KeyCode.S)) {
            movement.z = -1;
        }
        if (Input.GetKey(KeyCode.D)) {
            movement.x = 1;
        }
        frontTarget.Translate(movement * 5f * Time.deltaTime);
        movement = movement.normalized;
        frontTarget.forward = -movement;
    }
    private void OnDrawGizmos() {
        if (Application.isPlaying) {
            Gizmos.DrawCube(pathPointsPositions[0], Vector3.one * 0.25f);
            Gizmos.DrawCube(pathPointsPositions[1], Vector3.one * 0.5f);
            Gizmos.DrawCube(pathPointsPositions[2], Vector3.one * 0.75f);
        }
    }
}
