using UnityEngine;

public class MoveTargetCollisionPlane : MonoBehaviour
{
    [SerializeField] private Transform target;
    private void Update() {
        transform.position = target.position;
    }
}
