using System.Runtime.CompilerServices;
using UnityEngine;

public class Mouth : MonoBehaviour
{
    private bool isEating;
    private float eatingProgress;
    private float eatingSpeed;
    private Food foodBeingEaten;
    [SerializeField] private float timeToEat;
    [SerializeField] private CaterpillarControl caterpillarControl;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startEatingSound;
    private void Awake() {
        eatingSpeed = 1f / timeToEat;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Food") {
            foodBeingEaten = other.GetComponent<Food>();
            audioSource.PlayOneShot(startEatingSound);
            isEating = true;
        }
    }
    private void Update() {
        // Food takes 'timeToEat' seconds to enter the mouth.
        if (isEating) {
            eatingProgress += eatingSpeed * Time.deltaTime;
            foodBeingEaten.transform.position = Vector3.Lerp(foodBeingEaten.transform.position, transform.position, eatingProgress);
            foodBeingEaten.transform.localScale = Vector3.Lerp(foodBeingEaten.transform.localScale, Vector3.zero, eatingProgress);
            // Once food is done entering the mouth, destroy it and start growing caterpillar size.
            if (eatingProgress >= 1f) {
                Destroy(foodBeingEaten.gameObject);
                animator.Play("Eat");
                float percentageFactor = 1f + foodBeingEaten.GetPercentSizeIncrease() / 100f;
                caterpillarControl.StartToGrowCaterpillarSize(percentageFactor);
                eatingProgress = 0f;
                foodBeingEaten = null;
                isEating = false;
            }
        }
    }
}
