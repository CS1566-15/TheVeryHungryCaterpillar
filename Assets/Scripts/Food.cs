using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] private float percentSizeIncrease;
    [SerializeField] private Type foodType;
    private CaterpillarControl caterpillarControl;
    private enum Type {
        Strawberry,
        Orange,
        Blueberry,
        Chocolate,
        Grape,
        Cake,
        PoisonFruit,
        Seed
    }
    private void Awake() {
        caterpillarControl = GameObject.FindGameObjectWithTag("Player").GetComponent<CaterpillarControl>();
    }
    public void UseAbility() {
        switch (foodType) {
            case Type.Strawberry:
                caterpillarControl.IncreaseNumberOfJumps();
                break;
            case Type.Orange:

                break;
            case Type.Blueberry:

                break;
            case Type.Chocolate:

                break;
            case Type.Grape:

                break;
            case Type.Cake:

                break;
            case Type.PoisonFruit:
                // Does nothing special, just punishes caterpillar by reducing size.
                break;
            case Type.Seed:
                // Also does nothing special, just grows caterpillar size.
                break;
            default:
                break;
        }
    }
    public float GetPercentSizeIncrease() {
        return percentSizeIncrease;
    }
}
