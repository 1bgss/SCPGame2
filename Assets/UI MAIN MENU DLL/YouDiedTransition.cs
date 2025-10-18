using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class YouDiedTransition : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;           // Gambar hitam untuk fade (UI Image full screen)
    public float fadeDuration = 0.8f; // Kecepatan fade (0.8 detik)
    public string youDiedScene = "YouDied";
    public string mainMenuScene = "MainMenu";
    public float delayBeforeMainMenu = 2f; // Tunggu 2 detik di layar You Died

    private bool isDying = false;

    // Fungsi ini dipanggil saat player mati
    public void PlayerDied()
    {
        if (isDying) return;
        isDying = true;
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Fade out cepat
        yield return StartCoroutine(Fade(1));

        // Pindah ke scene You Died
        SceneManager.LoadScene(youDiedScene);

        // Tunggu sedikit biar scene You Died muncul
        yield return new WaitForSeconds(0.5f);

        // Fade in (biar muncul You Died)
        yield return StartCoroutine(Fade(0));

        // Tunggu beberapa detik di layar You Died
        yield return new WaitForSeconds(delayBeforeMainMenu);

        // Fade out lagi
        yield return StartCoroutine(Fade(1));

        // Pindah ke Main Menu
        SceneManager.LoadScene(mainMenuScene);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeImage == null) yield break;

        fadeImage.gameObject.SetActive(true);
        Color c = fadeImage.color;
        float startAlpha = c.a;
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        fadeImage.color = c;
    }
}
