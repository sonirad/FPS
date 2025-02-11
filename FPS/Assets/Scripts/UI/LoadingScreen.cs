using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    private float currentProgress = 0.0f;
    private float targetProgress = 0.0f;
    private Slider slider;
    private TextMeshProUGUI loadingText;
    private TextMeshProUGUI completeText;
    private TextMeshProUGUI pressText;

    private string[] loadingString =
    {
        "Loading .", "Loading . .", "Loading . . ."
    };

    private float CurrentProgress
    {
        get => currentProgress;
        set
        {
            currentProgress = Mathf.Min(targetProgress, value);
            slider.value = currentProgress;

            if (currentProgress > 0.9999f)
            {
                OnLoadingComplete();
            }
        }
    }

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        loadingText = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(1);
        completeText = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(2);
        pressText = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(3);
        slider = child.GetComponent<Slider>();
        slider.value = 0.0f;
    }

    private void Update()
    {
        CurrentProgress += Time.deltaTime;
    }

    public void Initialize()
    {
        CurrentProgress = 0.0f;
        targetProgress = 0.5f;

        StartCoroutine(TextCoroutine());
    }

    private IEnumerator TextCoroutine()
    {
        int index = 0;

        while (true)
        {
            loadingText.text = loadingString[index];
            index = (index + 1) % loadingString.Length;

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void OnLoadingProgress(float progress)
    {
        targetProgress = progress;
        Debug.Log($"Progress : {progress}");
    }

    private void OnLoadingComplete()
    {
        loadingText.gameObject.SetActive(false);
        completeText.gameObject.SetActive(true);
        pressText.gameObject.SetActive(true);

        slider.value = 1;

        StopAllCoroutines();
    }
}
