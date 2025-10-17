using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public string sceneToLoad = "x";    // Scene tujuan saat Play
    public CanvasGroup fadePanel;            // Referensi panel fade
    public float fadeDuration = 1.5f;        // Lama fade

    void Start()
    {
        // Pastikan fade awal transparan
        if (fadePanel != null)
            fadePanel.alpha = 0;
    }

    public void OnPlayButton()
    {
        StartCoroutine(FadeAndLoad());
    }

    public void OnTutorialButton()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void OnExitButton()
    {
        StartCoroutine(FadeAndExit());
    }

    private IEnumerator FadeAndLoad()
    {
        yield return StartCoroutine(FadeIn());
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator FadeAndExit()
    {
        yield return StartCoroutine(FadeIn());
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // biar bisa berhenti di editor
#endif
    }

    private IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (fadePanel != null)
                fadePanel.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
    }
}
