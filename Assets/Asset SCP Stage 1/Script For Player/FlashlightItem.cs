using UnityEngine;

public class FlashlightItem : MonoBehaviour
{
    public static FlashlightItem instance;

    [Header("Referensi Flashlight")]
    public GameObject playerFlashlight; // flashlight di Main Camera
    public Sprite icon;                 // icon untuk inventory
    public float pickupRange = 2.0f;
    public int flashlightSlotIndex = 0; // slot khusus flashlight (index 0 = slot pertama)

    [Header("Debug / Optional")]
    public bool autoEquipForDebug = false; // kalau mau test langsung muncul

    [HideInInspector] public bool isHeld = false;

    private bool canTake = false;
    private bool isEquipped = false;  // sedang di tangan
    private bool isOn = false;        // status lampu

    private Transform player;
    private Collider col;
    private Rigidbody rb;

    void Awake()
    {
        instance = this;

        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // pastikan flashlight main mati dulu
        if (playerFlashlight != null)
        {
            playerFlashlight.SetActive(false);
            var light = playerFlashlight.GetComponentInChildren<Light>();
            if (light != null) light.enabled = false;
        }
    }

    void Update()
    {
        if (player == null) return;
        float distance = Vector3.Distance(player.position, transform.position);

        // ===== AMBIL (E) =====
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakeFlashlight();

        // ===== CEK SLOT AKTIF =====
        if (isHeld)
        {
            int activeSlot = InventoryManager.instance.activeSlot;

            // equip saat slot aktif = slot flashlight
            if (activeSlot == flashlightSlotIndex && !isEquipped)
                EquipFlashlight();
            else if (activeSlot != flashlightSlotIndex && isEquipped)
                UnequipFlashlight();
        }

        // ===== TOGGLE LAMPU (F) =====
        if (isHeld && isEquipped && Input.GetKeyDown(KeyCode.F))
            ToggleFlashlight();

        // ===== DROP (G) =====
        if (isHeld && Input.GetKeyDown(KeyCode.G))
        {
            if (InventoryManager.instance.activeSlot == flashlightSlotIndex)
                DropFlashlight();
        }
    }

    // =====================================================
    // AMBIL ITEM DARI DUNIA
    // =====================================================
    private void TakeFlashlight()
    {
        bool added = InventoryManager.instance.AddItem(icon);
        if (!added)
        {
            Debug.Log("❌ Inventory penuh!");
            return;
        }

        isHeld = true;
        gameObject.SetActive(false);
        if (col != null) col.enabled = false;

        Debug.Log("✅ Flashlight diambil dan masuk inventory.");

        // debug langsung equip jika opsi aktif
        if (autoEquipForDebug)
            EquipFlashlight();
    }

    // =====================================================
    // EQUIP / UNEQUIP PUBLIC
    // =====================================================
    public void EquipFlashlight()
    {
        if (!isHeld || playerFlashlight == null) return;

        playerFlashlight.SetActive(true);
        var light = playerFlashlight.GetComponentInChildren<Light>();
        if (light != null) light.enabled = false; // masih off dulu

        isEquipped = true;
        isOn = false;
        Debug.Log("🔦 Flashlight muncul di tangan player (OFF).");
    }

    public void UnequipFlashlight()
    {
        if (playerFlashlight != null)
        {
            var light = playerFlashlight.GetComponentInChildren<Light>();
            if (light != null) light.enabled = false;
            playerFlashlight.SetActive(false);
        }

        isEquipped = false;
        isOn = false;
        Debug.Log("🔦 Flashlight disembunyikan dari tangan player.");
    }

    // =====================================================
    // TOGGLE LAMPU (F)
    // =====================================================
    public void ToggleFlashlight()
    {
        if (!isEquipped || playerFlashlight == null) return;

        var light = playerFlashlight.GetComponentInChildren<Light>();
        if (light == null) return;

        isOn = !isOn;
        light.enabled = isOn;
        Debug.Log(isOn ? "💡 Flashlight ON" : "🌑 Flashlight OFF");
    }

    // =====================================================
    // DROP ITEM
    // =====================================================
    public void DropFlashlight()
    {
        if (!isHeld) return;

        // Matikan flashlight player
        UnequipFlashlight();

        // Munculkan kembali di dunia
        transform.position = player.position + player.forward * 0.8f + Vector3.up * 0.3f;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
        if (col != null) col.enabled = true;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
        }

        isHeld = false;

        InventoryManager.instance.RemoveItem(icon);
        InventoryManager.instance.SetActiveSlot(-1);

        var toggle = FindFirstObjectByType<InventoryToggle>();
        if (toggle != null)
            toggle.ClearHighlights();

        Debug.Log("🗑️ Flashlight dijatuhkan ke tanah dan keluar dari inventory.");
    }

    // =====================================================
    // DETEKSI PLAYER
    // =====================================================
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

    // =====================================================
    // SLOT DOUBLE CLICK (UI)
    // =====================================================
    public void OnSlotDoubleClicked(int slotIndex)
    {
        if (slotIndex != flashlightSlotIndex || !isHeld) return;

        if (!isEquipped)
            EquipFlashlight();
        else
            UnequipFlashlight();
    }
}
