using UnityEngine;
using System.Collections;

public class RunPotion : MonoBehaviour
{
    public static RunPotion instance;

    [Header("Referensi Potion")]
    public GameObject playerPotionObject;
    public Sprite icon;
    public float pickupRange = 2f;
    public int slotIndex = 3; // misal slot 4
    public float effectDuration = 10f;

    [HideInInspector] public bool isHeld = false;

    private bool canTake = false;
    private bool isEffectActive = false;

    private Transform player;
    private Collider col;
    private PlayerMovement movement;

    void Awake()
    {
        instance = this;
        col = GetComponent<Collider>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            movement = player.GetComponent<PlayerMovement>();

        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;
        float distance = Vector3.Distance(player.position, transform.position);

        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakePotion();

        if (isHeld && InventoryManager.instance.activeSlot == slotIndex && Input.GetKeyDown(KeyCode.F))
            UsePotion();
    }

    private void TakePotion()
    {
        if (!InventoryManager.instance.AddItem(icon)) return;

        isHeld = true;
        if (col != null) col.enabled = false;
        gameObject.SetActive(false);

        Debug.Log("✅ Run Potion diambil dan masuk inventory.");
    }

    public void UsePotion()
    {
        if (!isHeld || isEffectActive || movement == null) return;

        isEffectActive = true;
        if (playerPotionObject != null)
            playerPotionObject.SetActive(true);

        StartCoroutine(ApplyRunEffect());
    }

    private IEnumerator ApplyRunEffect()
    {
        float originalRun = movement.runSpeed;
        movement.runSpeed *= 2f;
        Debug.Log("🏃‍♂️ Run Potion aktif: lari cepat 10 detik");

        yield return new WaitForSeconds(effectDuration);

        movement.runSpeed = originalRun;
        Debug.Log("🏃‍♂️ Run Potion habis");

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
