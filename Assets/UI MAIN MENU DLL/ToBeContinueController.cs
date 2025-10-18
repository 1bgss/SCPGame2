using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ToBeContinueController : MonoBehaviour
{
    public Image fadeImage; // UI Image hitam overlay
    public float fadeDuration = 2f;
    public float displayDuration = 3f;
    public string nextSceneName = "MainMenu"; // tujuan setelah ini

    private void Start()
    {
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // mulai dari layar hitam
        fadeImage.color = new Color(0, 0, 0, 1);

        // Fade in (buka layar)
        yield return StartCoroutine(FadeImage(fadeImage, 1, 0, fadeDuration));

        // Tahan beberapa detik sebelum fade out
        yield return new WaitForSeconds(displayDuration);

        // Fade out layar hitam
        yield return StartCoroutine(FadeImage(fadeImage, 0, 1, fadeDuration));

        // Pindah ke scene berikutnya
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeImage(Image img, float startAlpha, float endAlpha, float duration)
    {
        float t = 0f;
        Color c = img.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            img.color = c;
            yield return null;
        }

        c.a = endAlpha;
        img.color = c;
    }
}
