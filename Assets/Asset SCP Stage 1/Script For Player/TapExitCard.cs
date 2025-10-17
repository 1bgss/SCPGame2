using UnityEngine;
using System.Collections;

public class TapExitCard : MonoBehaviour
{
    [Header("Referensi Lampu & Pintu")]
    public GameObject greenLight;
    public GameObject redLight;
    public Transform doorTransform;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Waktu Reset Lampu")]
    public float lightResetDelay = 2f;

    private bool canTap = false;
    private bool doorOpened = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        // Simpan rotasi awal pintu (jika ada)
        if (doorTransform != null)
        {
            closedRotation = doorTransform.rotation;
            openRotation = doorTransform.rotation * Quaternion.Euler(0, openAngle, 0);
        }

        // Matikan semua lampu di awal
        if (greenLight != null) greenLight.SetActive(false);
        if (redLight != null) redLight.SetActive(false);

        Debug.Log($"[TapExitCard] Script aktif di object: {gameObject.name}");

        if (doorTransform == null)
            Debug.LogWarning("[TapExitCard] DoorTransform belum di-assign (pintu opsional)");

        // Pastikan collider benar
        Collider col = GetComponent<Collider>();
        if (col == null)
            Debug.LogError("[TapExitCard] ⚠️ Tidak ada Collider! Tambahkan Box Collider + centang Is Trigger");
        else if (!col.isTrigger)
            Debug.LogWarning("[TapExitCard] ⚠️ Collider belum Is Trigger = true");
    }

    void Update()
    {
        if (!canTap) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[TapExitCard] E ditekan di area trigger");

            bool hasExitCard = CheckExitCardInInventory();
            Debug.Log("[TapExitCard] Slot aktif berisi kartu? " + hasExitCard);

            if (hasExitCard)
            {
                if (greenLight != null) greenLight.SetActive(true);
                if (redLight != null) redLight.SetActive(false);
                StartCoroutine(ResetLights());

                if (doorTransform != null && !doorOpened)
                    StartCoroutine(OpenDoor());
            }
            else
            {
                if (redLight != null) redLight.SetActive(true);
                if (greenLight != null) greenLight.SetActive(false);
                StartCoroutine(ResetLights());
            }
        }
    }

    private IEnumerator ResetLights()
    {
        yield return new WaitForSeconds(lightResetDelay);
        if (greenLight != null) greenLight.SetActive(false);
        if (redLight != null) redLight.SetActive(false);
        Debug.Log("[TapExitCard] Lampu direset ke OFF");
    }

    // ✅ hanya cek slot aktif
    private bool CheckExitCardInInventory()
    {
        if (InventoryManager.instance == null)
        {
            Debug.LogWarning("[TapExitCard] InventoryManager.instance == null (belum ada di scene?)");
            return false;
        }

        int active = InventoryManager.instance.activeSlot;
        if (active < 0 || active >= InventoryManager.instance.slotIcons.Length)
        {
            Debug.Log("[TapExitCard] Tidak ada slot aktif!");
            return false;
        }

        var activeObj = InventoryManager.instance.GetItemObjectAtSlot(active);
        bool hasCard = (activeObj != null && activeObj is ExitCardItem);
        Debug.Log("[TapExitCard] Slot aktif (" + active + ") berisi ExitCard? " + hasCard);
        return hasCard;
    }

    private IEnumerator OpenDoor()
    {
        doorOpened = true;
        Debug.Log("[TapExitCard] Membuka pintu...");

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorTransform.rotation = Quaternion.Slerp(closedRotation, openRotation, t);
            yield return null;
        }

        Debug.Log("[TapExitCard] Pintu terbuka penuh, menunggu sebelum menutup...");
        yield return new WaitForSeconds(2f);

        // Tutup kembali
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorTransform.rotation = Quaternion.Slerp(openRotation, closedRotation, t);
            yield return null;
        }

        doorOpened = false;
        Debug.Log("[TapExitCard] Pintu ditutup kembali.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTap = true;
            Debug.Log("[TapExitCard] ✅ Player MASUK area trigger");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTap = false;
            Debug.Log("[TapExitCard] ⛔ Player KELUAR area trigger");
        }
    }
}
