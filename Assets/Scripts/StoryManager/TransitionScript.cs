using UnityEngine;
using System.Collections;

public class TransitionScript : MonoBehaviour
{
    [Range(0f, 1f)]
    public float maskValue = 0.135f;

    public float transitionDuration = 1f;
    private float targetValue;
    private Coroutine transitionCoroutine;

    public void CloseCurtain()
    {
        StartTransition(0.9f); // Шторка закрывается
    }

    public void OpenCurtain()
    {
        StartTransition(0.135f); // Шторка открывается
    }

    private void StartTransition(float toValue)
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(AnimateMask(toValue));
    }

    private IEnumerator AnimateMask(float toValue)
    {
        float fromValue = maskValue;
        float time = 0f;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = time / transitionDuration;
            maskValue = Mathf.Lerp(fromValue, toValue, t);
            yield return null;
        }

        maskValue = toValue;
    }
}
