using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class penamanExitDoorFadeOke : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;        // assign Image fullscreen hitam
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (fadeImage == null)
            Debug.LogError("[penamanExitDoorFadeOke] ⚠️ Fade Image belum di-assign!");
        else
            fadeImage.gameObject.SetActive(true);
        
        // Mulai dengan transparent
        SetAlpha(0f);
    }

    /// <summary>
    /// Panggil untuk fade out ke scene berikut
    /// </summary>
    public void FadeToNextScene(string nextScene)
    {
        StartCoroutine(FadeOutAndLoad(nextScene));
    }

    private IEnumerator FadeOutAndLoad(string nextScene)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            SetAlpha(Mathf.Lerp(0f, 1f, t));
            yield return null;
        }

        // Pastikan alpha 1 sebelum load
        SetAlpha(1f);

        // Load scene berikut
        SceneManager.LoadScene(nextScene);
    }

    /// <summary>
    /// Utility: ubah alpha image
    /// </summary>
    private void SetAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }
    }
}
