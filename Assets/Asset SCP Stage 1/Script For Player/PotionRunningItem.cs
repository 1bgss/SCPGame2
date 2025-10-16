using UnityEngine;
using System.Collections;

public class PotionRunningItem : MonoBehaviour
{
    public static PotionRunningItem instance;

    [Header("Referensi Potion")]
    public GameObject playerPotionObject; // muncul di tangan player
    public Sprite icon;
    public int potionSlotIndex = 2;       // slot UI (slot 3 = index 2)
    public float effectDuration = 3f;

    [Header("Debug")]
    public bool autoUseForDebug = false;

    [HideInInspector] public bool isHeld = false;

    private bool canTake = false;
    private bool isEffectActive = false;

    private Transform player;
    private Collider col;
    private PlayerMovement playerMovement;

    void Awake()
    {
        instance = this;
        col = GetComponent<Collider>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            playerMovement = player.GetComponent<PlayerMovement>();

        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;
        float distance = Vector3.Distance(player.position, transform.position);

        // Ambil potion
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= 2f && !isHeld)
            TakePotion();

        // Gunakan potion saat slot aktif & F
        if (isHeld && InventoryManager.instance.activeSlot == potionSlotIndex && Input.GetKeyDown(KeyCode.F))
            CheckUseInput();
    }

    private void TakePotion()
    {
        if (!InventoryManager.instance.AddItem(icon)) return;

        isHeld = true;
        if (col != null) col.enabled = false;

        // jangan nonaktifkan world object supaya coroutine bisa jalan
        // gameObject.SetActive(false);

        Debug.Log("✅ Potion running diambil dan masuk inventory.");

        if (autoUseForDebug)
            CheckUseInput();
    }

    private void CheckUseInput()
    {
        if (!isHeld || isEffectActive || playerMovement == null) return;

        isEffectActive = true;
        if (playerPotionObject != null)
            playerPotionObject.SetActive(true);

        // Jalankan coroutine dari player (selalu aktif)
        playerMovement.StartCoroutine(ApplySpeedEffect());
    }

    private IEnumerator ApplySpeedEffect()
    {
        float originalWalk = playerMovement.walkSpeed;
        float originalRun = playerMovement.runSpeed;

        // Speed boost
        playerMovement.walkSpeed *= 2f;
        playerMovement.runSpeed *= 2f;

        Debug.Log("⚡ Potion running aktif!");

        yield return new WaitForSeconds(effectDuration);

        // Reset speed
        playerMovement.walkSpeed = originalWalk;
        playerMovement.runSpeed = originalRun;

        Debug.Log("⚡ Potion running habis.");

        // Hapus dari inventory
        InventoryManager.instance.RemoveItem(icon);

        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);

        isHeld = false;
        isEffectActive = false;

        Destroy(gameObject);
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
