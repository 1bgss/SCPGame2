using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Fade Settings")]
    public CanvasGroup fadeCanvas;   // drag UI image FadeScreen ke sini
    public float fadeSpeed = 2f;     // kecepatan fade (semakin besar semakin cepat)
    
    [Header("Scene Settings")]
    public string youDiedScene = "YouDied"; // nama scene "YouDied"
    
    private bool isDead = false;

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 Player mati! Memulai fade ke scene YouDied...");
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        if (fadeCanvas == null)
        {
            Debug.LogWarning("⚠️ Fade canvas belum diset!");
            SceneManager.LoadScene(youDiedScene);
            yield break;
        }

        fadeCanvas.blocksRaycasts = true; // biar input stop
        fadeCanvas.interactable = false;

        // Fade IN ke hitam
        while (fadeCanvas.alpha < 1f)
        {
            fadeCanvas.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // Setelah fade selesai, ganti scene
        SceneManager.LoadScene(youDiedScene);
    }
}
