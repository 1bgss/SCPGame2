using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public string sceneToLoad = "x";  // nama scene tujuan
    public Image fadeOverlay;         // image hitam untuk efek fade
    public float fadeDuration = 1f;

    private bool isFading = false;

    // Dipanggil dari tombol Play
    public void OnPlayButton()
    {
        if (isFading)
        {
            Debug.Log("⏳ Masih fading, klik diabaikan.");
            return;
        }

        Debug.Log("🎮 Tombol Play diklik!");
        StartCoroutine(FadeOutAndLoad(sceneToLoad));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;

        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            float t = 0;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(0, 1, t / fadeDuration);
                fadeOverlay.color = c;
                yield return null;
            }
        }

        Debug.Log("🌑 Fade selesai, ganti ke scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
