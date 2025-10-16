using UnityEngine;
using System.Collections;

public class RunPotionItem : MonoBehaviour
{
    public static RunPotionItem instance;

    [Header("Referensi Potion")]
    public GameObject playerPotionObject;
    public Sprite icon;
    public float pickupRange = 2f;
    public float effectDuration = 10f;
    public float speedMultiplier = 2f;

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
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakePotion();

        // Gunakan potion
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
        // Kirim reference object ke InventoryManager
        if (!InventoryManager.instance.AddItem(icon, this)) return;

        isHeld = true;
        if (col != null) col.enabled = false;
        gameObject.SetActive(false);

        Debug.Log("⚡ Run Potion diambil dan masuk inventory.");
    }

    public void UsePotion(int slotIndex)
    {
        if (!isHeld || isEffectActive || playerMovement == null) return;

        isEffectActive = true;
        if (playerPotionObject != null)
            playerPotionObject.SetActive(true);

        // Jalankan efek speed
        playerMovement.StartCoroutine(ApplyRunEffect(slotIndex));
    }

    private IEnumerator ApplyRunEffect(int slotIndex)
    {
        float originalRun = playerMovement.runSpeed;

        playerMovement.runSpeed *= speedMultiplier;
        Debug.Log("🏃‍♂️ Run Potion aktif!");

        yield return new WaitForSeconds(effectDuration);

        // Reset kecepatan
        playerMovement.runSpeed = originalRun;
        Debug.Log("🏃‍♂️ Run Potion habis");

        // Hapus dari inventory
        InventoryManager.instance.RemoveItem(icon);
        InventoryManager.instance.ClearSlot(slotIndex);

        // Matikan model di tangan
        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);

        isHeld = false;
        isEffectActive = false;

        Destroy(gameObject);
    }

    public void OnSlotDoubleClicked(int slot)
    {
        if (!isHeld) return;
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
