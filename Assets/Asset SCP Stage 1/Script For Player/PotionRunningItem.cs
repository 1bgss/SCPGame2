using UnityEngine;
using System.Collections;

public class PotionRunningItem : MonoBehaviour
{
    public static PotionRunningItem instance;

    [Header("Referensi Potion")]
    public GameObject playerPotionObject; // model potion di tangan
    public Sprite icon;                    // icon untuk inventory
    public float pickupRange = 2f;
    public float effectDuration = 5f;      // durasi efek speed
    public float speedMultiplier = 2f;     // multiplier kecepatan

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

        // ===== AMBIL POTION (E) =====
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakePotion();

        // ===== GUNAKAN POTION (F) =====
        if (isHeld && !isEffectActive && Input.GetKeyDown(KeyCode.F))
        {
            int activeSlot = InventoryManager.instance.activeSlot;
            Sprite slotSprite = InventoryManager.instance.GetItemAtSlot(activeSlot);

            if (slotSprite == icon)
                UsePotion(activeSlot);
        }
    }

    private void TakePotion()
    {
        // ✅ Kirim 'this' supaya reference object tersimpan di InventoryManager
        if (!InventoryManager.instance.AddItem(icon, this)) return;

        isHeld = true;
        if (col != null) col.enabled = false;
        gameObject.SetActive(false);

        Debug.Log("⚡ Potion Running diambil dan masuk inventory.");
    }

    public void UsePotion(int slotIndex)
    {
        if (!isHeld || isEffectActive || playerMovement == null) return;

        isEffectActive = true;
        if (playerPotionObject != null)
            playerPotionObject.SetActive(true);

        // Jalankan efek dari player agar coroutine tetap jalan walau potion dihapus
        playerMovement.StartCoroutine(ApplySpeedEffect());
    }

    private IEnumerator ApplySpeedEffect()
    {
        float originalWalk = playerMovement.walkSpeed;
        float originalRun = playerMovement.runSpeed;

        // Aktifkan efek speed
        playerMovement.walkSpeed *= speedMultiplier;
        playerMovement.runSpeed *= speedMultiplier;
        Debug.Log("💨 Potion Running aktif!");

        yield return new WaitForSeconds(effectDuration);

        // Reset kecepatan normal
        playerMovement.walkSpeed = originalWalk;
        playerMovement.runSpeed = originalRun;
        Debug.Log("🏃‍♂️ Efek Potion Running habis, kecepatan normal lagi.");

        // Hapus dari inventory
        InventoryManager.instance.RemoveItem(icon);
        InventoryManager.instance.SetActiveSlot(-1);

        // Matikan model di tangan
        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);

        isHeld = false;
        isEffectActive = false;

        // Hapus objek potion dari dunia
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) canTake = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) canTake = false;
    }

    public void OnSlotDoubleClicked(int slotIndex)
    {
        // sembunyikan potion di tangan saat slot double click
        if (!isHeld) return;
        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);
    }
}
