using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SpiritIconClick : MonoBehaviour, IPointerClickHandler
{
    [Header("Fade Settings")]
    public Image fadeImage;           // Drag UI Image hitam ke sini
    public float fadeDuration = 1f;   // Durasi fade (dalam detik)
    public string sceneToLoad = "SceneX"; // Ganti sesuai nama scene target

    private bool isFading = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFading) return;

        Debug.Log("🌀 Spirit Icon diklik!");
        StartCoroutine(FadeOutAndLoad());
    }

    private IEnumerator FadeOutAndLoad()
    {
        isFading = true;

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;

            float t = 0f;

            // Naikkan alpha dari 0 ke 1 (layar gelap)
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
                fadeImage.color = c;
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("🔄 Memuat scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}
