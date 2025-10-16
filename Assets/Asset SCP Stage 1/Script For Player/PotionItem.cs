using UnityEngine;
using System.Collections;

public class PotionItem : MonoBehaviour
{
    public static PotionItem instance;

    [Header("Referensi Potion")]
    public GameObject playerPotionObject; // model potion di tangan player
    public Sprite icon;                    // icon untuk inventory
    public float pickupRange = 2f;
    public float effectDuration = 8f;      // durasi efek potion (detik)

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

        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;
        float distance = Vector3.Distance(player.position, transform.position);

        // Ambil potion dengan E
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakePotion();

        // Gunakan potion dengan F jika icon-nya aktif di slot sekarang
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
        // ✅ Tambahkan potion ke inventory dengan reference object
        if (!InventoryManager.instance.AddItem(icon, this)) return;

        isHeld = true;
        if (col != null) col.enabled = false;
        gameObject.SetActive(false);

        Debug.Log("🧃 Potion diambil dan masuk inventory.");
    }

    public void UsePotion(int slotIndex)
    {
        if (!isHeld || isEffectActive || stamina == null) return;

        isEffectActive = true;
        if (playerPotionObject != null)
            playerPotionObject.SetActive(true);

        // Jalankan efek dari player supaya coroutine tidak terhenti
        stamina.StartCoroutine(ApplyStaminaEffect(slotIndex));
    }

    private IEnumerator ApplyStaminaEffect(int slotIndex)
    {
        float originalMax = stamina.maxStamina;
        float originalDrain = stamina.staminaDrain;

        // Efek unlimited stamina
        stamina.maxStamina = 9999f;
        stamina.staminaDrain = 0f;
        Debug.Log("🔥 Potion aktif: stamina unlimited!");

        yield return new WaitForSeconds(effectDuration);

        // Reset stamina normal
        stamina.maxStamina = originalMax;
        stamina.staminaDrain = originalDrain;
        Debug.Log("💔 Efek potion habis, stamina kembali normal.");

        // 🔻 Hapus dari inventory secara pasti
        InventoryManager.instance.ClearSlot(slotIndex);
        InventoryManager.instance.SetActiveSlot(-1);

        // Matikan model di tangan
        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);

        isHeld = false;
        isEffectActive = false;

        // Hancurkan object potion
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

    public void OnSlotDoubleClicked(int slotIndex)
    {
        // sembunyikan potion di tangan saat slot double click
        if (!isHeld) return;
        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);
    }
}
