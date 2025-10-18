using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ExitButtonClick : MonoBehaviour, IPointerClickHandler
{
    [Header("Fade Settings")]
    public Image fadeImage;           // drag UI image hitam di sini
    public float fadeDuration = 1f;   // durasi fade
    private bool isFading = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isFading)
        {
            Debug.Log("❌ Tombol Exit diklik!");
            StartCoroutine(FadeOutAndExit());
        }
        else
        {
            Debug.Log("⏳ Masih fading, klik diabaikan.");
        }
    }

    private IEnumerator FadeOutAndExit()
    {
        isFading = true;
        float t = 0f;
        Color c = fadeImage.color;

        // pastikan fadeImage aktif
        fadeImage.gameObject.SetActive(true);

        // alpha 0 → 1 (gelap)
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        // jeda dikit biar fade-nya selesai
        yield return new WaitForSeconds(0.2f);

        Debug.Log("💤 Keluar dari game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
