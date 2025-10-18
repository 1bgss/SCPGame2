using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenManager : MonoBehaviour
{
    public CanvasGroup fadeCanvas;      // drag canvas group utama (background hitam)
    public CanvasGroup youDiedImage;    // drag canvas group untuk gambar “You Died”
    public float fadeDuration = 1f;     // waktu fade hitam
    public float showDuration = 2f;     // waktu tampilan “You Died”
    public string mainMenuScene = "MainMenu"; // nama scene main menu kamu

    private bool isFading = false;

    void Start()
    {
        fadeCanvas.alpha = 0f;
        youDiedImage.alpha = 0f;
    }

    public void ShowDeathScreen()
    {
        if (!isFading)
            StartCoroutine(DeathSequence());
    }

    private System.Collections.IEnumerator DeathSequence()
    {
        isFading = true;

        // Fade ke hitam
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        // Munculkan “You Died”
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            youDiedImage.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        // Tahan beberapa detik
        yield return new WaitForSeconds(showDuration);

        // Fade keluar lagi
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            youDiedImage.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        // Ganti ke Main Menu
        SceneManager.LoadScene(mainMenuScene);
    }
}
