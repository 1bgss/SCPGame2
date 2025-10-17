using UnityEngine;

public class ExitCardItem : MonoBehaviour
{
    [Header("Referensi Kartu")]
    public GameObject mainCardObject; // model di tangan player
    public Sprite icon;               // icon untuk inventory
    public float pickupRange = 2f;

    [HideInInspector] public bool isHeld = false;

    private Transform player;
    private bool canTake = false;
    private Collider col;

    // Posisi/rotasi/scale di tangan player
    private Vector3 handLocalPos = new Vector3(1.12f, -0.2554092f, 2.31f);
    private Vector3 handLocalRot = new Vector3(-75.066f, -87.416f, 0);
    private Vector3 handLocalScale = new Vector3(0.09f, 0.085f, 0.051f);

    void Awake()
    {
        col = GetComponent<Collider>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Tetap hide sampai slot diklik
        if (mainCardObject != null)
        {
            mainCardObject.SetActive(false);
            mainCardObject.transform.SetParent(player);
            mainCardObject.transform.localPosition = handLocalPos;
            mainCardObject.transform.localEulerAngles = handLocalRot;
            mainCardObject.transform.localScale = handLocalScale;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // Ambil kartu dengan E
        if (canTake && Input.GetKeyDown(KeyCode.E) && distance <= pickupRange && !isHeld)
            TakeCard();
    }

    private void TakeCard()
    {
        if (!InventoryManager.instance.AddItem(icon, this)) return;

        isHeld = true;
        gameObject.SetActive(false); // Destroy world object, mainCard tetap di tangan tapi hide

        Debug.Log("✅ Exit Card diambil dan masuk inventory!");
    }

    // Akan dipanggil dari InventoryToggle saat slot diklik
    public void ShowCardInHand()
    {
        if (mainCardObject != null)
            mainCardObject.SetActive(true);
    }

    public void HideCardInHand()
    {
        if (mainCardObject != null)
            mainCardObject.SetActive(false);
    }

    public void UseCard()
    {
        if (!isHeld) return;

        Debug.Log("🟢 Exit Card digunakan!");
        // Contoh: panggil fungsi pintu buka
        // ExitGate.instance.OpenDoor();
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
