using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader instance;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // tetap hidup antar scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    public void FadeToScene(string sceneName, float fadeTime = 0.3f)
    {
        StartCoroutine(FadeRoutine(sceneName, fadeTime));
    }

    private IEnumerator FadeRoutine(string sceneName, float fadeTime)
    {
        // Fade in ke hitam
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeTime);
            yield return null;
        }

        // Ganti scene
        SceneManager.LoadScene(sceneName);
    }
}
