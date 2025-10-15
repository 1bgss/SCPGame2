using UnityEngine;

public class FlashlightItem : MonoBehaviour
{
    [Header("Referensi Flashlight")]
    public GameObject playerFlashlight; // Flashlight di Main Camera
    public Light worldLight;            // Lampu di versi dunia (boleh null)

    [Header("Item Data")]
    public Sprite icon;                 // Icon inventory

    [Header("Slot Tetap Flashlight")]
    [SerializeField] private int flashlightSlotIndex = 0; // slot 1 = index 0

    [Header("Pengaturan Jarak Ambil")]
    [SerializeField] private float pickupRange = 2.0f;

    private bool canTake = false;
    private bool isHeld = false;      // sudah dimiliki
    private bool isEquipped = false;  // sedang di tangan
    private bool isOn = false;        // status lampu

    private Collider col;
    private Rigidbody rb;
    private Transform player;

    void Awake()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Pastikan dua-duanya mati dulu
        if (playerFlashlight != null)
        {
            playerFlashlight.SetActive(false);
            var light = playerFlashlight.GetComponentInChildren<Light>();
            if (light != null) light.enabled = false;
        }

        if (worldLight != null)
            worldLight.enabled = false;
    }

    void Update()
    {
        float distance = player ? Vector3.Distance(transform.position, player.position) : 999f;

        // ===== Ambil (E) =====
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakeFlashlight();

        // ===== Cek slot aktif untuk toggle equip =====
        if (isHeld)
        {
            int activeSlot = InventoryManager.instance.activeSlot;

            if (activeSlot == flashlightSlotIndex && !isEquipped)
                EquipFlashlight();
            else if (activeSlot != flashlightSlotIndex && isEquipped)
                UnequipFlashlight();
        }

        // ===== Toggle lampu (F) =====
        if (isHeld && isEquipped && Input.GetKeyDown(KeyCode.F))
            ToggleFlashlight();

        // ===== Drop (G) =====
        if (isHeld && Input.GetKeyDown(KeyCode.G))
        {
            if (InventoryManager.instance.activeSlot == flashlightSlotIndex)
                DropFlashlight();
            else
                Debug.Log("⚠️ Tidak bisa drop — slot aktif bukan flashlight.");
        }
    }

    // ===================================================
    // AMBIL
    // ===================================================
    private void TakeFlashlight()
    {
        bool added = InventoryManager.instance.AddItem(icon);
        if (!added)
        {
            Debug.Log("❌ Inventory penuh!");
            return;
        }

        isHeld = true;
        col.enabled = false;
        gameObject.SetActive(false);

        // Pastikan versi player mati dulu
        if (playerFlashlight != null)
        {
            playerFlashlight.SetActive(false);
            var light = playerFlashlight.GetComponentInChildren<Light>();
            if (light != null) light.enabled = false;
        }

        // Paksa aktifkan border slot 1
        InventoryManager.instance.SetActiveSlot(flashlightSlotIndex);
        var toggle = FindObjectOfType<InventoryToggle>();
        if (toggle != null)
            toggle.ForceHighlightSlot(flashlightSlotIndex);

        Debug.Log("✅ Flashlight diambil dan masuk ke inventory (slot 1).");
    }

    // ===================================================
    // EQUIP / UNEQUIP
    // ===================================================
    private void EquipFlashlight()
    {
        if (playerFlashlight == null) return;

        playerFlashlight.SetActive(true);
        var light = playerFlashlight.GetComponentInChildren<Light>();
        if (light != null) light.enabled = false;

        isEquipped = true;
        isOn = false;
        Debug.Log("🔦 Flashlight muncul di tangan player.");
    }

    private void UnequipFlashlight()
    {
        if (playerFlashlight != null)
        {
            var light = playerFlashlight.GetComponentInChildren<Light>();
            if (light != null) light.enabled = false;
            playerFlashlight.SetActive(false);
        }

        isEquipped = false;
        isOn = false;
        Debug.Log("🔦 Flashlight disembunyikan karena slot 1 nonaktif.");
    }

    // ===================================================
    // TOGGLE LAMPU
    // ===================================================
    private void ToggleFlashlight()
    {
        if (!isEquipped || playerFlashlight == null) return;

        var light = playerFlashlight.GetComponentInChildren<Light>();
        if (light == null) return;

        isOn = !isOn;
        light.enabled = isOn;
        Debug.Log(isOn ? "💡 Flashlight ON" : "🌑 Flashlight OFF");
    }

    // ===================================================
    // DROP
    // ===================================================
    private void DropFlashlight()
    {
        if (!isHeld) return;

        // Matikan flashlight player
        if (playerFlashlight != null)
        {
            var light = playerFlashlight.GetComponentInChildren<Light>();
            if (light != null) light.enabled = false;
            playerFlashlight.SetActive(false);
        }

        // Munculkan flashlight dunia di depan player
        transform.position = player.position + player.forward * 0.8f + Vector3.up * 0.4f;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
        col.enabled = true;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.down * 2f;
        }

        if (worldLight != null)
            worldLight.enabled = false;

        isHeld = false;
        isEquipped = false;
        isOn = false;

        InventoryManager.instance.RemoveItem(icon);
        InventoryManager.instance.SetActiveSlot(-1);

        var toggle = FindObjectOfType<InventoryToggle>();
        if (toggle != null)
            toggle.ClearHighlights();

        Debug.Log("🗑️ Flashlight dijatuhkan ke tanah dan keluar dari inventory.");
    }

    // ===================================================
    // DETEKSI PLAYER
    // ===================================================
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
