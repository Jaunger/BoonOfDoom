using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeLoadingScreenIcon : MonoBehaviour
{
    [SerializeField] Image fadeImage;
    private Coroutine fadeCoroutine;

    private void OnEnable()
    {
        FadeUIImage();
    }

    private void OnDisable()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
    }

    public void FadeUIImage()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeInOut(true));
    }

    private IEnumerator FadeInOut(bool fadeAway)
    {
        if(fadeAway)
        {
            for (float i = 1; i >= 0; i -= Time.unscaledDeltaTime)
            {
                fadeImage.color = new Color(1,1,1,i);
                yield return null;
            }

            fadeCoroutine = StartCoroutine(FadeInOut(false));
        }
        else
        {
            for (float i = 0; i <= 0; i += Time.unscaledDeltaTime)
            {
                fadeImage.color = new Color(1, 1, 1, i);
                yield return null;
            }

            fadeCoroutine = StartCoroutine(FadeInOut(true));
        }
    }
}
