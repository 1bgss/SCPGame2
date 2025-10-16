using UnityEngine;
using System.Collections;

public class StaminaPotion : MonoBehaviour
{
    public static StaminaPotion instance;

    [Header("Referensi Potion")]
    public GameObject playerPotionObject; // main potion di tangan player
    public Sprite icon;
    public float pickupRange = 2f;
    public int slotIndex = 2; // misal slot 3
    public float effectDuration = 10f;

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

        // PICKUP
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakePotion();

        // USE
        if (isHeld && InventoryManager.instance.activeSlot == slotIndex && Input.GetKeyDown(KeyCode.F))
            UsePotion();
    }

    private void TakePotion()
    {
        if (!InventoryManager.instance.AddItem(icon)) return;

        isHeld = true;
        if (col != null) col.enabled = false;
        gameObject.SetActive(false);

        Debug.Log("✅ Stamina Potion diambil dan masuk inventory.");
    }

    public void UsePotion()
    {
        if (!isHeld || isEffectActive || stamina == null) return;

        isEffectActive = true;
        if (playerPotionObject != null)
            playerPotionObject.SetActive(true);

        StartCoroutine(ApplyStaminaEffect());
    }

    private IEnumerator ApplyStaminaEffect()
    {
        float originalMax = stamina.maxStamina;
        float originalDrain = stamina.staminaDrain;

        stamina.maxStamina = 9999f;
        stamina.staminaDrain = 0f;
        Debug.Log("💚 Stamina Potion aktif: unlimited 10 detik");

        yield return new WaitForSeconds(effectDuration);

        stamina.maxStamina = originalMax;
        stamina.staminaDrain = originalDrain;
        Debug.Log("💔 Stamina Potion habis");

        InventoryManager.instance.RemoveItem(icon);
        InventoryManager.instance.SetActiveSlot(-1);

        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);

        isHeld = false;
        isEffectActive = false;

        Destroy(gameObject);
    }

    public void OnSlotDoubleClicked(int slot)
    {
        if (slot != slotIndex || !isHeld) return;
        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);
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
