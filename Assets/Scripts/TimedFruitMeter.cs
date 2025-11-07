using UnityEngine;

public class TimedFruitMeter : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;
    private void Update() {
        transform.position = Camera.main.WorldToScreenPoint(targetToFollow.position) + new Vector3(100, 100, 0);
    }
}
