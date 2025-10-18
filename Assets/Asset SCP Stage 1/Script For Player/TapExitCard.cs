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

    [Header("Scene Transition")]
    public penamanExitDoorFadeOke fadeController; // drag Canvas Fade baru
    public string nextSceneName = "SpiritIconToBeContinue"; // nama scene berikut

    private bool canTap = false;
    private bool doorOpened = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        // Set posisi awal dan posisi terbuka pintu
        if (doorTransform != null)
        {
            closedRotation = doorTransform.rotation;
            openRotation = doorTransform.rotation * Quaternion.Euler(0, openAngle, 0);
        }

        // Matikan lampu awal
        if (greenLight != null) greenLight.SetActive(false);
        if (redLight != null) redLight.SetActive(false);

        Debug.Log($"[TapExitCard] Script aktif di object: {gameObject.name}");

        // Cek collider
        Collider col = GetComponent<Collider>();
        if (col == null)
            Debug.LogError("[TapExitCard] ⚠️ Tidak ada Collider! Tambahkan Box Collider + centang Is Trigger");
        else if (!col.isTrigger)
            Debug.LogWarning("[TapExitCard] ⚠️ Collider belum Is Trigger = true");
    }

    void Update()
    {
        if (!canTap) return;

        // Tombol E untuk menggunakan kartu
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[TapExitCard] E ditekan di area trigger");

            bool hasExitCard = CheckExitCardInInventory();
            Debug.Log("[TapExitCard] Slot aktif berisi kartu? " + hasExitCard);

            if (hasExitCard)
            {
                // Lampu hijau ON
                if (greenLight != null) greenLight.SetActive(true);
                if (redLight != null) redLight.SetActive(false);
                StartCoroutine(ResetLights());

                // Buka pintu
                if (doorTransform != null && !doorOpened)
                    StartCoroutine(OpenDoor());
            }
            else
            {
                // Lampu merah ON
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

        Debug.Log("[TapExitCard] Pintu terbuka penuh, menunggu sebelum transisi...");

        yield return new WaitForSeconds(2f);

        // 🔹 Fade ke scene berikut menggunakan penamanExitDoorFadeOke
        if (fadeController != null)
        {
            fadeController.FadeToNextScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("[TapExitCard] ⚠️ FadeController belum di-assign, transisi dilewati.");
        }

        // (Opsional) kalau ingin pintu menutup setelah fade
        // t = 0;
        // while (t < 1f)
        // {
        //     t += Time.deltaTime * openSpeed;
        //     doorTransform.rotation = Quaternion.Slerp(openRotation, closedRotation, t);
        //     yield return null;
        // }
        // doorOpened = false;
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
