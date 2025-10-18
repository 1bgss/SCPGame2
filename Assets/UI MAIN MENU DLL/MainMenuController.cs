using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeOverlay;       // UI Image full screen hitam
    public float fadeDuration = 1f; // durasi fade masuk/keluar
    public string playScene = "SceneX"; // scene tujuan Play
    public string exitScene = "";       // jika mau exit pakai fungsi Application.Quit()

    private bool isFading = false;

    void Start()
    {
        // Pastikan overlay aktif dan menutupi layar awal
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeOverlay.raycastTarget = true;
            fadeOverlay.color = new Color(0, 0, 0, 1); // hitam penuh
            StartCoroutine(FadeIn());
        }

        // Cursor muncul saat MainMenu aktif
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Fade in dari hitam → transparan
    private IEnumerator FadeIn()
    {
        float t = 0f;
        Color c = fadeOverlay.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadeOverlay.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeOverlay.color = c;
        fadeOverlay.raycastTarget = false; // biar tombol bisa diklik
    }

    // Dipanggil dari tombol Play
    public void OnPlayButton()
    {
        if (!isFading)
        {
            StartCoroutine(FadeOutAndLoad(playScene));
        }
    }

    // Dipanggil dari tombol Exit
    public void OnExitButton()
    {
        if (!isFading)
        {
            // Bisa pakai fade juga kalau mau
            Application.Quit();
            Debug.Log("Game keluar!");
        }
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;

        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeOverlay.raycastTarget = true;

            Color c = fadeOverlay.color;
            float t = 0f;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
                fadeOverlay.color = c;
                yield return null;
            }

            c.a = 1f;
            fadeOverlay.color = c;
        }

        SceneManager.LoadScene(sceneName);
    }
}
