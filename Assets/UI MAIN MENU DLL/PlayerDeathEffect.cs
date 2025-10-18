using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerDeathEffect : MonoBehaviour
{
    public float fadeDelay = 0.5f;
    public string youDiedScene = "YouDied";

    private bool isDead = false;

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("☠️ Player mati – transisi ke scene YouDied...");

        // Matikan semua kontrol
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour s in scripts)
        {
            if (s != this) s.enabled = false;
        }

        // Hentikan physics supaya ga jatuh ke void
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Sedikit rotasi rebah (opsional)
        transform.rotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);

        StartCoroutine(LoadYouDiedScene());
    }

    IEnumerator LoadYouDiedScene()
    {
        yield return new WaitForSeconds(fadeDelay);
        SceneManager.LoadScene(youDiedScene);
    }
}
