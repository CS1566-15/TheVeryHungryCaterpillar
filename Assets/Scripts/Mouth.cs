using System.Runtime.CompilerServices;
using UnityEngine;

public class Mouth : MonoBehaviour
{
    private Transform foodBeingEaten;
    private bool isEating;
    private float eatingProgress;
    private float eatingSpeed;
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
            foodBeingEaten = other.transform;
            audioSource.PlayOneShot(startEatingSound);
            isEating = true;
        }
    }
    private void Update() {
        if (isEating) {
            eatingProgress += eatingSpeed * Time.deltaTime;
            foodBeingEaten.position = Vector3.Lerp(foodBeingEaten.position, transform.position, eatingProgress);
            foodBeingEaten.localScale = Vector3.Lerp(foodBeingEaten.localScale, Vector3.zero, eatingProgress);
            if (eatingProgress >= 1f) {
                Destroy(foodBeingEaten.gameObject);
                animator.Play("Eat");
                caterpillarControl.StartToGrowCaterpillarSize();
                eatingProgress = 0f;
                foodBeingEaten = null;
                isEating = false;
            }
        }
    }
}
