using UnityEngine;
using TMPro;
using System.Collections;

public class ExitCardItem : MonoBehaviour
{
    [Header("Referensi Kartu")]
    public GameObject mainCardObject; // model di tangan player
    public Sprite icon;
    public float pickupRange = 2f;

    [Header("Suara Pickup")]
    public AudioSource audioSource;
    public AudioClip pickupSound;

    [Header("UI Prompt")]
    public TextMeshProUGUI promptText;
    public float promptRange = 2f;
    private float hideRange = 2.5f;
    public float fadeDuration = 0.3f;

    [HideInInspector] public bool isHeld = false;

    private Transform player;
    private bool canTake = false;
    private bool promptVisible = false;
    private Coroutine fadeRoutine;
    private Collider col;

    private readonly Vector3 handLocalPos = new(1.12f, -0.2554092f, 2.31f);
    private readonly Vector3 handLocalRot = new(-75.066f, -87.416f, 0f);
    private readonly Vector3 handLocalScale = new(0.09f, 0.085f, 0.051f);

    private void Awake()
    {
        col = GetComponent<Collider>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Setup kartu di tangan
        if (mainCardObject != null)
        {
            mainCardObject.SetActive(false);
            mainCardObject.transform.SetParent(player);
            mainCardObject.transform.localPosition = handLocalPos;
            mainCardObject.transform.localEulerAngles = handLocalRot;
            mainCardObject.transform.localScale = handLocalScale;
        }

        // Pastikan audio source ada
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Inisialisasi teks transparan (alpha = 0)
        if (promptText != null)
        {
            Color c = promptText.color;
            c.a = 0;
            promptText.color = c;
            promptText.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        // Kalau udah diambil (world hilang), jangan lanjut cek jarak
        if (player == null || isHeld) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // --- fade prompt stabil ---
        if (!promptVisible && distance <= promptRange)
        {
            FadePrompt(true);
            promptVisible = true;
        }
        else if (promptVisible && distance > hideRange)
        {
            FadePrompt(false);
            promptVisible = false;
        }

        // --- pickup ---
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakeCard();
    }

    private void FadePrompt(bool show)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeTextAlpha(show));
    }

    private IEnumerator FadeTextAlpha(bool fadeIn)
    {
        if (promptText == null) yield break;

        float startAlpha = promptText.color.a;
        float endAlpha = fadeIn ? 1f : 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, t);

            Color c = promptText.color;
            c.a = newAlpha;
            promptText.color = c;

            yield return null;
        }

        Color finalColor = promptText.color;
        finalColor.a = endAlpha;
        promptText.color = finalColor;
    }

    private void TakeCard()
    {
        // Tambah ke inventory
        if (!InventoryManager.instance.AddItem(icon, this))
            return;

        isHeld = true;

        // Sembunyikan prompt dulu biar fade-out rapi
        FadePrompt(false);

        // Delay dikit sebelum disable object biar fade sempat kelar
        StartCoroutine(DisableAfterFade());

        // Update objective
        if (ObjectiveManager.instance != null)
            ObjectiveManager.instance.SetObjective("Go to the Exit Door");

        // Suara pickup
        if (pickupSound != null && audioSource != null)
            audioSource.PlayOneShot(pickupSound, 1f);

        Debug.Log("✅ Exit Card diambil dan masuk inventory!");
    }

    private IEnumerator DisableAfterFade()
    {
        yield return new WaitForSeconds(fadeDuration + 0.05f);
        gameObject.SetActive(false); // baru disable setelah fade selesai
    }

    public void ShowCardInHand()
    {
        if (mainCardObject != null)
            mainCardObject.SetActive(true);
    }

    public void HideCardInHand()
    {
        if (mainCardObject != null)
            mainCardObject.SetActive(false);
    }

    public void UseCard()
    {
        if (!isHeld) return;
        Debug.Log("🟢 Exit Card digunakan!");
        // Misalnya panggil pintu buka
        // ExitGate.instance.OpenDoor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            canTake = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTake = false;
    }
}
