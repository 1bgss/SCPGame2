using UnityEngine;
using TMPro;

public class ExitCardItem : MonoBehaviour
{
    [Header("Referensi Kartu")]
    [Tooltip("Model kartu yang muncul di tangan player.")]
    public GameObject mainCardObject;

    [Tooltip("Icon kartu untuk inventory UI.")]
    public Sprite icon;

    [Tooltip("Jarak maksimum agar bisa mengambil kartu.")]
    public float pickupRange = 2f;


    [Header("Suara Pickup")]
    [Tooltip("Audio source untuk memutar suara pickup.")]
    public AudioSource audioSource;

    [Tooltip("Suara yang dimainkan saat kartu diambil.")]
    public AudioClip pickupSound;


    [Header("UI Prompt")]
    [Tooltip("Text TMP yang muncul saat pemain mendekati kartu.")]
    public TextMeshProUGUI promptText;

    [Tooltip("Jarak maksimum agar teks prompt muncul (meter).")]
    public float promptRange = 0.01f;  // ±1 cm


    [HideInInspector] public bool isHeld = false;

    private Transform player;
    private bool canTake = false;
    private bool promptShown = false;
    private Collider col;

    // Posisi/rotasi/scale kartu di tangan player
    private readonly Vector3 handLocalPos = new(1.12f, -0.2554092f, 2.31f);
    private readonly Vector3 handLocalRot = new(-75.066f, -87.416f, 0f);
    private readonly Vector3 handLocalScale = new(0.09f, 0.085f, 0.051f);

    // ---------------------------------------------------
    // Lifecycle
    // ---------------------------------------------------
    private void Awake()
    {
        col = GetComponent<Collider>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Sembunyikan kartu di tangan saat awal
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

        // Sembunyikan prompt text di awal
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (player == null || isHeld) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // Tampilkan / sembunyikan prompt berdasarkan jarak
        if (distance <= promptRange && !promptShown)
        {
            if (promptText != null)
                promptText.gameObject.SetActive(true);

            promptShown = true;
        }
        else if (distance > promptRange && promptShown)
        {
            if (promptText != null)
                promptText.gameObject.SetActive(false);

            promptShown = false;
        }

        // Ambil kartu dengan tombol E
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakeCard();
    }

    // ---------------------------------------------------
    // Core Logic
    // ---------------------------------------------------
    private void TakeCard()
    {
        if (!InventoryManager.instance.AddItem(icon, this))
            return;

        isHeld = true;
        gameObject.SetActive(false); // Hilangkan world object

        if (promptText != null)
            promptText.gameObject.SetActive(false); // Hide text permanen

        // Update objective setelah ambil kartu
        if (ObjectiveManager.instance != null)
            ObjectiveManager.instance.SetObjective("Go to the Exit Door");

        // Mainkan suara pickup
        if (pickupSound != null && audioSource != null)
            audioSource.PlayOneShot(pickupSound, 1f);

        Debug.Log("✅ Exit Card diambil dan masuk inventory!");
    }

    // ---------------------------------------------------
    // Inventory Event Hooks
    // ---------------------------------------------------
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
        // Contoh: panggil fungsi pintu buka
        // ExitGate.instance.OpenDoor();
    }

    // ---------------------------------------------------
    // Trigger Detection
    // ---------------------------------------------------
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
