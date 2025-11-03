using System.Runtime.CompilerServices;
using UnityEngine;

public class JumpPathGenerator : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;
    [SerializeField] private GameObject pathObject;
    [SerializeField] private float speed;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private float heightMultiplier;
    [SerializeField] private AnimationCurve sizeCurve;
    [SerializeField] private float sizeMultiplier;
    [SerializeField] private int amount;
    [SerializeField] private float timeIntervalBetweenEachPathObject;

    private GameObject[] pathObjects;
    private float[] pathObjectsStartTimes;
    public void ShowPath() {
        for (int i = 0; i < amount; i++) {
            pathObjects[i].SetActive(true);
        }
    }
    public void HidePath() {
        for (int i = 0; i < amount; i++) {
            pathObjects[i].SetActive(false);
        }
    }
    private void Awake() {
        pathObjects = new GameObject[amount];
        pathObjectsStartTimes = new float[amount];
        for (int i = 0; i < amount; i++) {
            pathObjects[i] = Instantiate(pathObject);
            pathObjects[i].transform.position = start.position;
            pathObjectsStartTimes[i] = timeIntervalBetweenEachPathObject * i;
        }
        HidePath();
    }
    private void Update() {
        for (int i = 0; i < amount; i++) {
            float progress = Mathf.Clamp((Time.time - pathObjectsStartTimes[i]) * speed, 0f, 1f);
            pathObjects[i].transform.position = Vector3.Lerp(start.position, end.position, progress);
            pathObjects[i].transform.position = new Vector3(pathObjects[i].transform.position.x, pathObjects[i].transform.position.y + heightCurve.Evaluate(progress) * heightMultiplier, pathObjects[i].transform.position.z);
            pathObjects[i].transform.localScale = Vector3.one * sizeCurve.Evaluate(progress) * sizeMultiplier;
            if (progress >= 1f) {
                pathObjects[i].transform.position = start.position;
                pathObjectsStartTimes[i] = Time.time + timeIntervalBetweenEachPathObject;
            }
        }
    }
}
