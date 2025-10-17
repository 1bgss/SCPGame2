using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public CanvasGroup fadeImage;
    public float fadeDuration = 1f;
    public string nextSceneName = "MainMenu";

    private bool isFading = false;

    void Start()
    {
        // Awal: layar hitam, fade in ke video
        if (fadeImage != null) fadeImage.alpha = 1f;

        StartCoroutine(FadeIn());

        // Saat video selesai, panggil fungsi untuk fade out dan ganti scene
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.Play();
        }
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (fadeImage != null) fadeImage.alpha = 1f - (t / fadeDuration);
            yield return null;
        }
        if (fadeImage != null) fadeImage.alpha = 0f;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (!isFading)
            StartCoroutine(FadeOutAndLoad());
    }

    IEnumerator FadeOutAndLoad()
    {
        isFading = true;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (fadeImage != null) fadeImage.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        SceneManager.LoadScene(nextSceneName);
    }
}
