using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Fade Settings")]
    public Image fadeImage;              // Drag UI Image hitam ke sini (sama seperti tombol Play)
    public float fadeDuration = 1f;      // Durasi fade (bisa disamain)
    public string creditSceneName = "CreditScene"; // Nama scene tujuan (belum dibuat, nanti kamu buat)

    private bool isFading = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFading) return;

        Debug.Log("🎬 Tombol Credit diklik!");
        StartCoroutine(FadeAndLoadCredit());
    }

    private IEnumerator FadeAndLoadCredit()
    {
        isFading = true;

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;

            float t = 0f;

            // Naikkan alpha dari 0 → 1 (fade out)
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
                fadeImage.color = c;
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("🔄 Memuat scene credit: " + creditSceneName);
        SceneManager.LoadScene(creditSceneName);
    }
}
