using UnityEngine;
using System.Collections;

public class StaminaPotionItem : MonoBehaviour
{
    public static StaminaPotionItem instance;

    [Header("Referensi Potion")]
    public GameObject playerPotionObject; // model potion di tangan
    public Sprite icon;
    public float effectDuration = 8f;

    [HideInInspector] public bool isHeld = false;

    private bool canTake = false;
    private bool isEffectActive = false;
    private Transform player;
    private Collider col;
    private PlayerStamina playerStamina;

    void Awake()
    {
        instance = this;
        col = GetComponent<Collider>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            playerStamina = player.GetComponent<PlayerStamina>();

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

        // Gunakan potion (slot aktif)
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
        // ✅ Perubahan utama: kirim 'this' supaya reference object tersimpan
        if (!InventoryManager.instance.AddItem(icon, this)) return;

        isHeld = true;
        if (col != null) col.enabled = false;
        gameObject.SetActive(false);

        Debug.Log("🧃 Potion Unlimited diambil dan masuk inventory.");
    }

    public void UsePotion(int slotIndex)
    {
        if (!isHeld || isEffectActive || playerStamina == null) return;

        isEffectActive = true;
        if (playerPotionObject != null)
            playerPotionObject.SetActive(true);

        // Jalankan efek dari Player supaya tidak ke-cancel
        playerStamina.StartCoroutine(ApplyStaminaEffect(slotIndex));
    }

    private IEnumerator ApplyStaminaEffect(int slotIndex)
    {
        float originalMax = playerStamina.maxStamina;
        float originalDrain = playerStamina.staminaDrain;

        // Efek unlimited stamina
        playerStamina.maxStamina = 9999f;
        playerStamina.staminaDrain = 0f;
        Debug.Log("🔥 Potion Unlimited aktif!");

        yield return new WaitForSeconds(effectDuration);

        // Reset stamina normal
        playerStamina.maxStamina = originalMax;
        playerStamina.staminaDrain = originalDrain;
        Debug.Log("🧃 Efek habis, stamina kembali normal.");

        // Hapus dari inventory
        InventoryManager.instance.RemoveItem(icon);
        InventoryManager.instance.SetActiveSlot(-1);

        // Matikan model di tangan
        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);

        isHeld = false;
        isEffectActive = false;

        // Destroy setelah selesai
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
}
