using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ExitGateFadeController : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage; // drag Image hitam di Canvas
    public float fadeDuration = 2f;

    private bool isFading = false;

    public void FadeToNextScene(string sceneName)
    {
        if (!isFading)
            StartCoroutine(FadeOut(sceneName));
    }

    private IEnumerator FadeOut(string sceneName)
    {
        isFading = true;
        float t = 0;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
