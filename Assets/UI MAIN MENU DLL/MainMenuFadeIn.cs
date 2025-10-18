using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuFadeIn : MonoBehaviour
{
    public Image fadePanel;       // panel hitam transparan (UI Image full screen)
    public float fadeDuration = 1.5f; // durasi fade-in (detik)

    void Start()
    {
        if (fadePanel != null)
        {
            // mulai dari warna hitam penuh
            fadePanel.color = new Color(0, 0, 0, 1);
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, 0); // pastikan benar-benar transparan di akhir
    }
}
