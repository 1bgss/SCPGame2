using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathEffect : MonoBehaviour
{
    [Header("Death Settings")]
    public string deathSceneName = "You died";
    public float delayBeforeDeath = 1f; // waktu jeda sebelum pindah scene
    public bool fadeOut = true;
    public CanvasGroup fadeCanvas; // optional kalau mau efek fade

    private bool isDead = false;

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Matikan semua kontrol player
        CharacterController controller = GetComponent<CharacterController>();
        if (controller) controller.enabled = false;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var s in scripts)
        {
            if (s != this) s.enabled = false;
        }

        // Optional: fade out sebelum pindah scene
        if (fadeOut && fadeCanvas != null)
        {
            StartCoroutine(FadeAndLoadScene());
        }
        else
        {
            Invoke(nameof(LoadDeathScene), delayBeforeDeath);
        }
    }

    private System.Collections.IEnumerator FadeAndLoadScene()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        LoadDeathScene();
    }

    private void LoadDeathScene()
    {
        SceneManager.LoadScene(deathSceneName);
    }
}
