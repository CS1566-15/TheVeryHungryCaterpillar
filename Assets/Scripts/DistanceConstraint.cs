using UnityEngine;

public class DistanceConstraint : MonoBehaviour
{
    [SerializeField] private GameObject constrainedObject;
    [SerializeField] private float distance;
    private void FixedUpdate() {
        constrainedObject.transform.position = transform.position + (constrainedObject.transform.position - transform.position).normalized * distance;
    }
}
