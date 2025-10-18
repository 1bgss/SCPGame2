using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Target & Movement")]
    public Transform player;
    public float chaseRange = 15f;
    public float attackRange = 2f;
    public float moveSpeed = 3.5f;

    [Header("Scene Settings")]
    public string youDiedScene = "You died";   // nama scene di Build Settings
    public float fadeSpeed = 2f;               // kecepatan fade

    private NavMeshAgent agent;
    private bool hasKilledPlayer = false;
    private CanvasGroup fadeCanvas;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // 🔧 Buat fade hitam overlay di layar (langsung runtime)
        GameObject fadeObj = new GameObject("FadeCanvas");
        Canvas canvas = fadeObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas = fadeObj.AddComponent<CanvasGroup>();
        fadeCanvas.alpha = 0f;

        GameObject bg = new GameObject("FadeBG");
        bg.transform.SetParent(fadeObj.transform, false);
        Image img = bg.AddComponent<Image>();
        img.color = Color.black;
        RectTransform rect = img.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void Update()
    {
        if (player == null || hasKilledPlayer) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= chaseRange && distance > attackRange)
            ChasePlayer();
        else if (distance <= attackRange)
            KillPlayer();
        else
            StopChasing();
    }

    void ChasePlayer()
    {
        if (agent == null) return;
        agent.isStopped = false;
        agent.speed = moveSpeed;
        agent.SetDestination(player.position);
    }

    void StopChasing()
    {
        if (agent == null) return;
        agent.isStopped = true;
    }

    void KillPlayer()
    {
        if (hasKilledPlayer) return;
        hasKilledPlayer = true;

        // 🔒 Hentikan semua kontrol & physics player
        if (agent != null) agent.isStopped = true;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // 🔥 Matikan semua script Player (biar ga respawn)
        MonoBehaviour[] allScripts = player.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
            script.enabled = false;

        // Jalankan fade dan pindah scene
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeCanvas.alpha = alpha;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(youDiedScene);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
