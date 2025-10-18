using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeSceneX : MonoBehaviour
{
    [Header("🎥 Pengaturan Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1.5f;
    public bool fadeInOnStart = true;

    void Start()
    {
        if (fadeCanvasGroup != null && fadeInOnStart)
        {
            fadeCanvasGroup.alpha = 1; // mulai dari gelap
            StartCoroutine(FadeIn());
        }
    }

    // Fade dari hitam → terang
    public IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = 1 - Mathf.Clamp01(time / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0;
    }

    // Fade dari terang → hitam
    public IEnumerator FadeOutAndDo(System.Action onComplete)
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(time / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1;
        onComplete?.Invoke();
    }

    // Opsional: panggil ini buat pindah scene dengan fade out
    public void FadeOutToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndDo(() => SceneManager.LoadScene(sceneName)));
    }
}
