using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ToBeContinueController : MonoBehaviour
{
    public Image fadeImage; // UI Image hitam overlay
    public Image toBeContinueImage; // Gambar “To Be Continued”
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
        toBeContinueImage.color = new Color(1, 1, 1, 0);

        // Fade in (buka layar)
        yield return StartCoroutine(FadeImage(fadeImage, 1, 0, fadeDuration));

        // Munculkan teks
        yield return StartCoroutine(FadeImage(toBeContinueImage, 0, 1, 1f));

        // Tahan beberapa detik
        yield return new WaitForSeconds(displayDuration);

        // Fade out semua
        yield return StartCoroutine(FadeImage(toBeContinueImage, 1, 0, 1f));
        yield return StartCoroutine(FadeImage(fadeImage, 0, 1, fadeDuration));

        // Pindah ke MainMenu
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
