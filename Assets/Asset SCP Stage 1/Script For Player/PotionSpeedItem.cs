using UnityEngine;
using System.Collections;

public class PotionSpeedItem : MonoBehaviour
{
    [Header("Referensi Potion")]
    public GameObject playerPotionObject; // model potion di tangan player
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
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakePotion();
    }

    private void TakePotion()
    {
        // Simpan reference di inventory
        if (!InventoryManager.instance.AddItem(icon, this)) return;

        isHeld = true;

        if (col != null) col.enabled = false;
        gameObject.SetActive(false);

        Debug.Log("⚡ Speed Potion diambil dan masuk inventory.");
    }

    // Dipanggil dari InventoryInputManager
    public void UsePotion()
    {
        if (!isHeld || isEffectActive || playerMovement == null) return;

        isEffectActive = true;
        if (playerPotionObject != null)
            playerPotionObject.SetActive(true);

        playerMovement.StartCoroutine(ApplySpeedEffect());
    }

    private IEnumerator ApplySpeedEffect()
    {
        float originalWalk = playerMovement.walkSpeed;
        float originalRun = playerMovement.runSpeed;

        playerMovement.walkSpeed *= speedMultiplier;
        playerMovement.runSpeed *= speedMultiplier;
        Debug.Log("💨 Speed Potion aktif!");

        yield return new WaitForSeconds(effectDuration);

        playerMovement.walkSpeed = originalWalk;
        playerMovement.runSpeed = originalRun;
        Debug.Log("💨 Speed Potion habis, kecepatan kembali normal.");

        InventoryManager.instance.RemoveItem(icon);
        InventoryManager.instance.SetActiveSlot(-1);

        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);

        isHeld = false;
        isEffectActive = false;

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
}
