using UnityEngine;
using System.Collections;

public class PotionRunningItem : MonoBehaviour
{
    public static PotionRunningItem instance;

    [Header("Referensi Potion")]
    public GameObject playerPotionObject; // object muncul di tangan
    public Sprite icon;                    
    public int potionSlotIndex = 2;        
    public float effectDuration = 3f;     

    [Header("Debug")]
    public bool autoUseForDebug = false;   

    [HideInInspector] public bool isHeld = false;

    private bool canTake = false;
    private bool isEffectActive = false;

    private Transform player;
    private Collider col;
    private PlayerMovement playerMovement; // script movement

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

        // Ambil potion dengan E
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= 2f && !isHeld)
            TakePotion();

        // Gunakan potion dengan F jika slot aktif
        if (isHeld && InventoryManager.instance.activeSlot == potionSlotIndex && Input.GetKeyDown(KeyCode.F))
            UsePotion();
    }

    private void TakePotion()
    {
        // Tambahkan ke inventory
        if (!InventoryManager.instance.AddItem(icon)) return;

        isHeld = true;
        if (col != null) col.enabled = false;
        gameObject.SetActive(false);

        Debug.Log("✅ Potion running diambil dan masuk inventory.");

        if (autoUseForDebug)
            UsePotion();
    }

    public void UsePotion()
    {
        if (!isHeld || isEffectActive || playerMovement == null) return;

        isEffectActive = true;

        if (playerPotionObject != null)
            playerPotionObject.SetActive(true);

        StartCoroutine(ApplySpeedEffect());
    }

    private IEnumerator ApplySpeedEffect()
    {
        float originalWalk = playerMovement.walkSpeed;
        float originalRun = playerMovement.runSpeed;

        // Tambah kecepatan sementara
        playerMovement.walkSpeed *= 2f;
        playerMovement.runSpeed *= 2f;

        Debug.Log("💨 Potion running aktif!");

        yield return new WaitForSeconds(effectDuration);

        // Reset kecepatan
        playerMovement.walkSpeed = originalWalk;
        playerMovement.runSpeed = originalRun;

        // Hapus dari inventory
        InventoryManager.instance.RemoveItem(icon);

        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);

        isHeld = false;
        isEffectActive = false;

        Destroy(gameObject); // hapus object di dunia
        Debug.Log("💔 Potion running habis.");
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
}
