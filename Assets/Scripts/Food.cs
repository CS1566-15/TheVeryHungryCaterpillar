using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    [SerializeField] private float percentSizeIncrease;
    [SerializeField] private float thresholdSize;
    [SerializeField] private Type foodType;
    private CaterpillarControl caterpillarControl;
    private Image timedFruitMeterGraphic;
    private Animation timedFruitMeterAnimation;
    private TMP_Text timedFruitMeterText;
    private bool timedEffect = false;
    private float effectDuration = 0f;
    private float elapsedTime = 0f;
    private bool hasBeenEaten = false;
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
        timedFruitMeterGraphic = GameObject.FindGameObjectWithTag("TimedFruitMeter").GetComponent<Image>();
        timedFruitMeterAnimation = GameObject.FindGameObjectWithTag("TimedFruitMeter").GetComponent<Animation>();
        timedFruitMeterText = GameObject.FindGameObjectWithTag("TimedFruitMeter").transform.GetChild(0).GetComponent<TMP_Text>();
    }
    public void UseAbility() {
        timedFruitMeterGraphic.fillAmount = 1f;

        switch (foodType) {
            case Type.Strawberry:
                caterpillarControl.IncreaseNumberOfJumps();
                break;
            case Type.Orange:
                timedEffect = true;
                effectDuration = 7f;
                timedFruitMeterText.SetText("Jump ↑");
                caterpillarControl.SetHasIncreasedJumpDistance(true);
                break;
            case Type.Blueberry:
                timedEffect = true;
                effectDuration = 5f;
                timedFruitMeterText.SetText("Speed ↑");
                caterpillarControl.SetSpeedMultiplier(true);
                break;
            case Type.Chocolate:
                // TODO
                break;
            case Type.Grape:
                timedEffect = true;
                effectDuration = 10f;
                timedFruitMeterText.SetText("Growth ↑");
                caterpillarControl.SetGrowthMultiplier(true);
                break;
            case Type.Cake:
                // TODO
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

    private void EndTimedAbility() {
        timedFruitMeterAnimation.Play("TimedFruitMeterHide");
        timedEffect = false;
        elapsedTime = 0f;

        switch (foodType) {
            case Type.Orange:
                caterpillarControl.SetHasIncreasedJumpDistance(false);
                break;
            case Type.Blueberry:
                caterpillarControl.SetSpeedMultiplier(false);
                break;
            case Type.Chocolate:
                caterpillarControl.SetGrowthMultiplier(false);
                break;
            case Type.Grape:

                break;
        }

    }

    private void Update() {
        if (timedEffect) {
            if (elapsedTime == 0f) {
                timedFruitMeterAnimation.Play("TimedFruitMeterShow");
            }
            elapsedTime += Time.deltaTime;
            timedFruitMeterGraphic.fillAmount = 1f - elapsedTime / effectDuration;
            if (elapsedTime >= effectDuration) {
                EndTimedAbility();
            }
        }
    }

    public float GetPercentSizeIncrease()
    {
        return percentSizeIncrease;
    }
    public void SetHasBeenEaten(bool state)
    {
        hasBeenEaten = state;
    }
    public bool GetHasBeenEaten()
    {
        return hasBeenEaten;
    }
    public float GetThresholdSize()
    {
        return thresholdSize;
    }
}
