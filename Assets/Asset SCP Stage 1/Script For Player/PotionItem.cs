using UnityEngine;
using System.Collections;

public class PotionItem : MonoBehaviour
{
    public static PotionItem instance;

    [Header("Referensi Potion")]
    public GameObject playerPotionObject; // main potion di tangan player
    public Sprite icon;                    // icon untuk inventory
    public float pickupRange = 2f;
    public float effectDuration = 10f;     // durasi efek potion
    public int assignedSlot = 0;           // slot khusus potion

    [Header("Debug / Optional")]
    public bool autoUseForDebug = false;   // langsung pakai saat pickup

    [HideInInspector] public bool isHeld = false;

    private bool canTake = false;
    private bool isEffectActive = false;

    private Transform player;
    private Collider col;

    private PlayerStamina stamina;

    void Awake()
    {
        instance = this;

        col = GetComponent<Collider>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            stamina = player.GetComponent<PlayerStamina>();

        // hide main potion di tangan
        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;
        float distance = Vector3.Distance(player.position, transform.position);

        // PICKUP dari dunia
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakePotion();

        if (isHeld)
        {
            // tampilkan main potion kalau slot aktif dan efek belum aktif
            if (InventoryManager.instance.activeSlot == assignedSlot && !isEffectActive)
            {
                if (playerPotionObject != null)
                    playerPotionObject.SetActive(true);

                // pakai potion dengan F
                if (Input.GetKeyDown(KeyCode.F))
                    UsePotion();
            }
            else
            {
                // hide main potion kalau slot bukan potion
                if (playerPotionObject != null)
                    playerPotionObject.SetActive(false);
            }
        }
    }

    // =====================================================
    // AMBIL POTION
    // =====================================================
    private void TakePotion()
    {
        bool added = InventoryManager.instance.AddItem(icon);
        if (!added)
        {
            Debug.Log("❌ Inventory penuh!");
            return;
        }

        isHeld = true;
        if (col != null) col.enabled = false;
        gameObject.SetActive(false);

        Debug.Log("✅ Potion diambil dan masuk inventory.");

        if (autoUseForDebug)
            UsePotion();
    }

    // =====================================================
    // PAKAI POTION
    // =====================================================
    public void UsePotion()
    {
        if (!isHeld || isEffectActive) return;
        if (stamina == null) return;

        isEffectActive = true;

        // tampilkan main potion
        if (playerPotionObject != null)
            playerPotionObject.SetActive(true);

        StartCoroutine(ApplyStaminaEffect());
    }

    private IEnumerator ApplyStaminaEffect()
    {
        float originalMax = stamina.maxStamina;
        stamina.maxStamina = 9999f; // unlimited
        Debug.Log("💚 Potion stamina aktif: unlimited 10 detik");

        yield return new WaitForSeconds(effectDuration);

        stamina.maxStamina = originalMax;
        Debug.Log("💔 Potion stamina habis");

        // hapus potion dari inventory + hide main potion
        InventoryManager.instance.RemoveItem(icon);
        InventoryManager.instance.SetActiveSlot(-1);

        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);

        isHeld = false;
        isEffectActive = false;

        Destroy(gameObject); // optional, world object juga hilang
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
    // DOUBLE CLICK SLOT (UI)
    // =====================================================
    public void OnSlotDoubleClicked(int slotIndex)
    {
        if (slotIndex != assignedSlot || !isHeld) return;
        UsePotion();
    }
}
