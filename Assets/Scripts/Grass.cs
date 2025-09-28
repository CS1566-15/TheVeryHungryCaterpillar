using UnityEngine;

public class Grass : MonoBehaviour
{
    //private GameObject[] grass;
    [SerializeField] private Transform player;
    [SerializeField] private float interactionDistance;
    private Quaternion initialRotation;
    private void Awake() {
        //grass = GameObject.FindGameObjectsWithTag("Grass");
        initialRotation = transform.rotation;
    }
    private void Update() {
        Vector3 directionToGrass = transform.position - player.position;
        float distance = directionToGrass.magnitude;
        Quaternion targetRotation;
        if (distance <= interactionDistance) {
            directionToGrass.Normalize();
            directionToGrass.y = 0;
            targetRotation = Quaternion.LookRotation(directionToGrass);
        }
        else {
            targetRotation = initialRotation;
        }
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * 5
        );
    }
}
