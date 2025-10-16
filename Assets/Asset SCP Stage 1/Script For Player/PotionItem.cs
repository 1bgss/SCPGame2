using UnityEngine;
using System.Collections;

public class PotionItem : MonoBehaviour
{
    public static PotionItem instance;

    [Header("Referensi Potion")]
    public GameObject playerPotionObject; // potion di tangan player (main)
    public Sprite icon;                    // icon untuk inventory
    public float pickupRange = 2f;
    public int potionSlotIndex = 2;        // slot 3 (index 2)
    public float effectDuration = 10f;     // durasi efek potion

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

        // hide main potion dulu
        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;
        float distance = Vector3.Distance(player.position, transform.position);

        // PICKUP dengan E
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakePotion();

        // USE dengan F jika potion di slot aktif
        if (isHeld && InventoryManager.instance.activeSlot == potionSlotIndex && Input.GetKeyDown(KeyCode.F))
            UsePotion();
    }

    private void TakePotion()
    {
        if (!InventoryManager.instance.AddItem(icon)) return;

        isHeld = true;
        if (col != null) col.enabled = false;
        gameObject.SetActive(false);

        Debug.Log("✅ Potion diambil dan masuk inventory.");

        if (autoUseForDebug)
            UsePotion();
    }

    public void UsePotion()
    {
        if (!isHeld || isEffectActive || stamina == null) return;

        isEffectActive = true;

        // tampilkan main potion
        if (playerPotionObject != null)
            playerPotionObject.SetActive(true);

        // mulai coroutine efek stamina unlimited
        StartCoroutine(ApplyStaminaEffect());
    }

    private IEnumerator ApplyStaminaEffect()
    {
        float originalMax = stamina.maxStamina;
        float originalDrain = stamina.staminaDrain;

        // Stamina unlimited
        stamina.maxStamina = 9999f;
        stamina.staminaDrain = 0f; // stamina tidak berkurang
        Debug.Log("💚 Potion stamina aktif: unlimited 10 detik");

        yield return new WaitForSeconds(effectDuration);

        // Reset stamina
        stamina.maxStamina = originalMax;
        stamina.staminaDrain = originalDrain;
        Debug.Log("💔 Potion stamina habis");

        // hapus potion dari inventory + hide main potion
        InventoryManager.instance.RemoveItem(icon);
        InventoryManager.instance.SetActiveSlot(-1);

        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);

        isHeld = false;
        isEffectActive = false;

        Destroy(gameObject); // hilangkan world object
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

    public void OnSlotDoubleClicked(int slotIndex)
    {
        if (slotIndex != potionSlotIndex || !isHeld) return;

        // hide main potion jika double click (belum dipakai)
        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);
    }
}
