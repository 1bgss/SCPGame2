using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CreditSceneController : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;            // Drag UI Image hitam dari Canvas
    public float fadeDuration = 1f;    // Lama efek fade masuk/keluar
    public float stayDuration = 3f;    // Lama tampilan credit sebelum balik
    public string mainMenuScene = "MainMenu"; // Ganti nama scene Main Menu kamu

    private void Start()
    {
        // Pastikan fadeImage aktif dan full hitam di awal
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }

        // Jalankan urutan efek
        StartCoroutine(CreditSequence());
    }

    private IEnumerator CreditSequence()
    {
        // 🔹 Fade-In (dari hitam ke terang)
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        // 🔹 Tampilkan credit selama beberapa detik
        yield return new WaitForSeconds(stayDuration);

        // 🔹 Fade-Out (dari terang ke hitam)
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        // 🔹 Pindah ke Main Menu
        SceneManager.LoadScene(mainMenuScene);
    }
}
