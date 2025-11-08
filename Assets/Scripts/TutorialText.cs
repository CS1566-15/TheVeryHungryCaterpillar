using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialText : MonoBehaviour
{
    [Header("References")]
    public TMPro.TextMeshProUGUI tutorialText;

    [Header("Flash Settings")]
    [Range(0.1f, 3f)]
    public float flashSpeed = 1.5f; // Speed of the flash animation
    
    [Range(0f, 1f)]
    public float minAlpha = 0.3f; // Minimum opacity when flashing
    
    [Range(0f, 1f)]
    public float maxAlpha = 1f; // Maximum opacity when flashing

    [Header("Fade Out Settings")]
    public float fadeOutDuration = 0.5f; // How long the fade out takes

    private bool hasClicked = false;
    private Coroutine flashCoroutine;

    void Start()
    {
        if (tutorialText == null)
        {
            tutorialText = GetComponent<TMPro.TextMeshProUGUI>();
        }

        // Start flashing animation
        flashCoroutine = StartCoroutine(FlashText());
    }

    void Update()
    {
        // Detect mouse click
        if (!hasClicked && Input.GetMouseButtonDown(0))
        {
            OnPlayerClick();
        }
    }

    void OnPlayerClick()
    {
        hasClicked = true;

        // Stop flashing
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        // Start fade out
        StartCoroutine(FadeOutAndDestroy());
    }

    IEnumerator FlashText()
    {
        while (true)
        {
            // Flash animation using sine wave for smooth transition
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, 
                (Mathf.Sin(Time.time * flashSpeed * Mathf.PI) + 1f) / 2f);
            
            Color color = tutorialText.color;
            color.a = alpha;
            tutorialText.color = color;

            yield return null;
        }
    }

    IEnumerator FadeOutAndDestroy()
    {
        float elapsedTime = 0f;
        Color startColor = tutorialText.color;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / fadeOutDuration);
            
            Color color = tutorialText.color;
            color.a = alpha;
            tutorialText.color = color;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}