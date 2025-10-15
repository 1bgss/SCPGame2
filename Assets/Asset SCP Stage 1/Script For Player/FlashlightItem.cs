
using UnityEngine;

public class FlashlightItem : MonoBehaviour
{
    [Header("Item Data")]
    public Sprite icon;

    [Header("Posisi Saat Dipegang")]
    public Transform handPos;
    public Vector3 holdPositionOffset = new Vector3(0, 0, 0);
    public Vector3 holdRotationOffset = new Vector3(0, 0, 0);

    [Header("Komponen Flashlight")]
    public Light flashlightLight;       // drag komponen Light (child) ke sini di Inspector

    private bool isOn = false;
    private bool canTake = false;
    private bool isHeld = false;

    void Update()
    {
        // ===== Ambil Item (E) =====
        if (canTake && Input.GetKeyDown(KeyCode.E) && !isHeld)
        {
            // Cek apakah masih ada slot kosong
            bool added = InventoryManager.instance.AddItem(icon);

            if (added)
            {
                PickUpFlashlight();
                Debug.Log("✅ Flashlight diambil dan dimasukkan ke inventory.");
            }
            else
            {
                Debug.Log("❌ Inventory penuh! Tidak bisa ambil flashlight.");
            }
        }

        // ===== Drop Item (G) =====
        if (isHeld && InventoryManager.instance.GetActiveItem() == icon && Input.GetKeyDown(KeyCode.G))
        {
            DropFlashlight();
            InventoryManager.instance.RemoveItem(icon);
            Debug.Log("🗑️ Flashlight dibuang dan dihapus dari inventory.");
        }

        // ===== Nyalakan / Matikan Flashlight (F) =====
        if (isHeld && Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }
    }

    private void ToggleFlashlight()
    {
        if (flashlightLight == null)
        {
            Debug.LogWarning("⚠️ Komponen Light belum di-assign!");
            return;
        }

        isOn = !isOn;
        flashlightLight.enabled = isOn;
        Debug.Log(isOn ? "💡 Flashlight ON" : "🌑 Flashlight OFF");
    }

    private void PickUpFlashlight()
    {
        if (handPos == null)
        {
            Debug.LogWarning("⚠️ HandPos belum di-assign di Inspector!");
            return;
        }

        transform.SetParent(handPos);
        transform.localPosition = holdPositionOffset;
        transform.localEulerAngles = holdRotationOffset;

        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        isHeld = true;
    }

    private void DropFlashlight()
    {
        transform.SetParent(null);

        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            transform.position = player.transform.position + player.transform.forward * 1f + Vector3.up * 0.5f;
        }

        // Matikan lampu ketika dibuang
        if (flashlightLight != null)
            flashlightLight.enabled = false;

        isHeld = false;
        isOn = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTake = true;
            Debug.Log("➡️ Bisa ambil flashlight (Player dekat item).");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTake = false;
            Debug.Log("⬅️ Player menjauh dari flashlight.");
        }
    }
}
