using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float cameraFollowSmoothing;
    [Header("References")]
    [SerializeField] private CaterpillarControl caterpillarControl;
    [SerializeField] private Transform caterpillarBack;
    [SerializeField] private Transform caterpillarFront;
    [SerializeField] private Transform moveTarget;
    private Vector3 displacement;
    private void Awake() {
        Vector3 average = Vector3.Lerp(caterpillarBack.position, caterpillarFront.position, 0.5f);
        displacement = transform.position - average;
    }
    private void Update() {
        Vector3 average = Vector3.zero;
        if (caterpillarControl.GetIsWaitingToJump()) {
            average = Vector3.Lerp(caterpillarFront.position, moveTarget.position, 0.5f);
        }
        else {
            average = Vector3.Lerp(caterpillarBack.position, caterpillarFront.position, 0.5f);
        }
        transform.position = Vector3.Lerp(transform.position, average + displacement, Time.deltaTime * cameraFollowSmoothing);
    }
}
