using UnityEngine;
using System.Collections;

public class PotionUnlimitedStamina : MonoBehaviour
{
    [Header("Referensi Potion")]
    public GameObject playerPotionObject; // model potion di tangan player
    public Sprite icon;                    // icon untuk inventory
    public float pickupRange = 2f;
    public float effectDuration = 8f;      // durasi efek stamina unlimited (detik)

    [HideInInspector] public bool isHeld = false;

    private bool canTake = false;
    private bool isEffectActive = false;
    private Transform player;
    private Collider col;
    private PlayerStamina playerStamina;

    void Awake()
    {
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

        // Ambil potion dengan E
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakePotion();

        // Gunakan potion dengan F
        if (isHeld && !isEffectActive && Input.GetKeyDown(KeyCode.F))
            UsePotion();
    }

    private void TakePotion()
    {
        // Tambahkan ke inventory dengan reference ke object ini
        if (!InventoryManager.instance.AddItem(icon, this)) return;

        isHeld = true;
        if (col != null) col.enabled = false;
        gameObject.SetActive(false);

        Debug.Log("🧃 Unlimited Stamina Potion diambil dan masuk inventory.");
    }

    public void UsePotion()
    {
        if (!isHeld || isEffectActive || playerStamina == null) return;

        isEffectActive = true;
        if (playerPotionObject != null)
            playerPotionObject.SetActive(true);

        // Jalankan efek stamina unlimited
        playerStamina.StartCoroutine(ApplyStaminaEffect());
    }

    private IEnumerator ApplyStaminaEffect()
    {
        float originalMax = playerStamina.maxStamina;
        float originalDrain = playerStamina.staminaDrain;

        // Efek stamina unlimited
        playerStamina.maxStamina = 9999f;
        playerStamina.staminaDrain = 0f;
        Debug.Log("💚 Potion digunakan: stamina unlimited!");

        yield return new WaitForSeconds(effectDuration);

        // Reset stamina normal
        playerStamina.maxStamina = originalMax;
        playerStamina.staminaDrain = originalDrain;
        Debug.Log("💔 Efek potion habis, stamina kembali normal.");

        // Clear slot inventory jika masih ada
        if (InventoryManager.instance != null)
        {
            for (int i = 0; i < InventoryManager.instance.slotIcons.Length; i++)
            {
                if (InventoryManager.instance.GetItemObjectAtSlot(i) == this)
                {
                    InventoryManager.instance.ClearSlot(i);
                    InventoryManager.instance.SetActiveSlot(-1);
                    break;
                }
            }
        }

        // Matikan model di tangan
        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);

        isHeld = false;
        isEffectActive = false;

        // Hancurkan object world potion
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
