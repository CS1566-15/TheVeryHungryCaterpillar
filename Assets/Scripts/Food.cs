using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] private float percentSizeIncrease;
    public float GetPercentSizeIncrease() {
        return percentSizeIncrease;
    }
}
