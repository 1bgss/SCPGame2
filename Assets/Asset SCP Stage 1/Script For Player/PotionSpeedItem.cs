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
    public AudioClip drinkSound;

    [HideInInspector] public bool isHeld = false;

    private bool canTake = false;
    private bool isEffectActive = false;
    private Transform player;
    private Collider col;
    private PlayerMovement playerMovement;

    [Header("Audio Source (assign di Player)")]
    public AudioSource potionAudioSource;

    void Awake()
    {
        col = GetComponent<Collider>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();

            // Pastikan AudioSource ada di Player
            if (potionAudioSource == null)
            {
                potionAudioSource = player.GetComponent<AudioSource>();
                if (potionAudioSource == null)
                {
                    potionAudioSource = player.gameObject.AddComponent<AudioSource>();
                    potionAudioSource.name = "PotionSpeedAudioSource";
                }
            }
        }

        if (playerPotionObject != null)
            playerPotionObject.SetActive(false); // model di tangan mulai nonaktif
    }

    void Update()
    {
        if (player == null) return;
        float distance = Vector3.Distance(player.position, transform.position);

        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakePotion();
    }

    private void TakePotion()
    {
        if (!InventoryManager.instance.AddItem(icon, this)) return;

        isHeld = true;

        if (col != null) col.enabled = false;

        // Aman: world potion tetap di-disable, model di tangan tetap muncul saat dipakai
        gameObject.SetActive(false);

        Debug.Log("⚡ Speed Potion diambil dan masuk inventory.");
    }

    public void UsePotion()
    {
        if (!isHeld || isEffectActive || playerMovement == null) return;

        // Mainkan suara dari Player
        if (drinkSound != null && potionAudioSource != null)
            potionAudioSource.PlayOneShot(drinkSound);

        isEffectActive = true;

        // Tampilkan model potion di tangan player
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
            playerPotionObject.SetActive(false); // sembunyikan model di tangan

        isHeld = false;
        isEffectActive = false;

        Destroy(gameObject); // hapus object world
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
