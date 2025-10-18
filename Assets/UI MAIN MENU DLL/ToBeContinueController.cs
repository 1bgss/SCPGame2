using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ToBeContinueController : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;               // UI Image full screen hitam
    public float fadeDuration = 2f;       // durasi fade in/out
    public float displayDuration = 3f;    // durasi menunggu layar terbuka
    public string nextSceneName = "MainMenu"; // nama scene tujuan

    private void Start()
    {
        // Pastikan canvas aktif dan raycast aktif saat fade
        fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true;

        // Hitam penuh awal
        fadeImage.color = new Color(0,0,0,1);

        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // --- Fade in (hitam -> transparan) ---
        yield return StartCoroutine(FadeImage(fadeImage, 1f, 0f, fadeDuration));

        // Tahan layar terbuka
        yield return new WaitForSeconds(displayDuration);

        // --- Fade out (transparan -> hitam) ---
        yield return StartCoroutine(FadeImage(fadeImage, 0f, 1f, fadeDuration));

        // Nonaktifkan raycast agar UI MainMenu bisa diklik
        fadeImage.raycastTarget = false;
        fadeImage.gameObject.SetActive(false);

        // Pastikan cursor muncul di MainMenu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pindah ke scene MainMenu
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
