using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    public Image bar;
    
    [SerializeField] private Gradient gradient;
    [SerializeField] private float drainTime = 0.2f;

    private float targetValue = 1;
    private Color targetColour;

    private Coroutine drainBarCoroutine;
    [SerializeField] private bool coroutineRunning;

    void Start()
    {
        bar.color = gradient.Evaluate(targetValue);
    }

    private void Update()
    {
        transform.rotation = new Quaternion(0.5f, 0, 0, 0.866025388f);
    }

    public void SetFillAmount(float value)
    {
        targetValue = value;
        targetColour = gradient.Evaluate(targetValue);

        // If the change is drastic we add more time to the transition.
        if (bar.fillAmount - value > 0.25) drainTime += 0.2f;
        if (bar.fillAmount - value > 0.5) drainTime += 0.2f;
        if (bar.fillAmount - value > 0.75) drainTime += 0.2f;

        // If we take multiple hits, don't call again instead raise the total time for the transition.
        if (!coroutineRunning) drainBarCoroutine = StartCoroutine(DrainBar());
        else
        {
            drainTime += 0.2f;
        }
    }

    private IEnumerator DrainBar()
    {
        float elapsedTime = 0;
        float fillAmount = bar.fillAmount;
        Color currentColour = bar.color;

        while (elapsedTime < drainTime)
        {
            coroutineRunning = true;
            elapsedTime += Time.deltaTime;

            bar.fillAmount = Mathf.Lerp(fillAmount, targetValue, elapsedTime / drainTime);
            bar.color = Color.Lerp(currentColour, targetColour, elapsedTime / drainTime);

            yield return null;
        }

        // Reset values.
        coroutineRunning = false;
        drainTime = 0.2f;
    }
}
