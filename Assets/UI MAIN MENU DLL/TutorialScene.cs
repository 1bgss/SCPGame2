using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialScene : MonoBehaviour
{
    [Header("UI Tutorial")]
    public Image tutorialImage;
    public float displayTime = 5f;
    public float fadeTime = 1f;

    [Header("UI Gameplay yang mau disembunyikan")]
    public GameObject[] gameplayUI; // drag: InventoryCanvas, ObjectiveCanvas, StaminaCanvas, dll.

    [Header("Player Camera")]
    public WakeUpEffect wakeUpEffect; // drag kamera atau object yg ada script WakeUpEffect

    private CanvasGroup canvasGroup;

    void Start()
    {
        // Matikan dulu semua UI gameplay
        foreach (GameObject ui in gameplayUI)
            ui.SetActive(false);

        canvasGroup = tutorialImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = tutorialImage.gameObject.AddComponent<CanvasGroup>();

        StartCoroutine(TutorialRoutine());
    }

    IEnumerator TutorialRoutine()
    {
        // Munculkan tutorial
        canvasGroup.alpha = 1;
        yield return new WaitForSeconds(displayTime);

        // Fade out tutorial
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeTime);
            yield return null;
        }

        tutorialImage.gameObject.SetActive(false);

        // Setelah tutorial selesai → aktifkan UI gameplay
        foreach (GameObject ui in gameplayUI)
            ui.SetActive(true);

        // 🔹 Mulai efek bangun
        if (wakeUpEffect != null)
            wakeUpEffect.StartWakeUp();

        Debug.Log("✅ Tutorial selesai, UI gameplay aktif + efek bangun dimulai!");
    }
}
