using UnityEngine;

public class ImprovedCaterpillar : MonoBehaviour
{
    private Vector3[] pathPoints = new Vector3[3];
    [SerializeField] private Transform[] IKTargets;
    [SerializeField] private float distanceBetweenPathPoints;
    private void Awake() {
        pathPoints[0] = IKTargets[0].position;
        pathPoints[1] = IKTargets[1].position;
        pathPoints[2] = IKTargets[2].position;
    }
    private void Update() {
        float distanceFromFrontPathPoint = Vector3.Distance(transform.position, pathPoints[0]);
        if (distanceFromFrontPathPoint >= distanceBetweenPathPoints) {
            pathPoints[2] = pathPoints[1];
            pathPoints[1] = pathPoints[0];
            pathPoints[0] = transform.position;


        }
        for (int i = 0; i < 3; i++) {
            IKTargets[i].position = Vector3.Lerp(IKTargets[i].position, pathPoints[i], Time.deltaTime * 5f);
        }
    }
    private void OnDrawGizmos() {
        if (Application.isPlaying) {
            Gizmos.DrawCube(pathPoints[0], Vector3.one * 0.25f);
            Gizmos.DrawCube(pathPoints[1], Vector3.one * 0.5f);
            Gizmos.DrawCube(pathPoints[2], Vector3.one * 0.75f);
        }
    }
}
