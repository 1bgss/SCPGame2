using UnityEngine;
using System.Collections;
using TMPro; // ✅ pakai TextMeshPro

public class TapExitCard : MonoBehaviour
{
    [Header("Referensi Lampu & Pintu")]
    public GameObject greenLight;
    public GameObject redLight;
    public Transform doorTransform;

    [Header("Pengaturan Pintu")]
    public float openAngle = 90f;
    public bool invertRotation = false;
    public float openSpeed = 2f;

    [Header("Waktu Reset Lampu")]
    public float lightResetDelay = 2f;

    [Header("Scene Transition")]
    public penamanExitDoorFadeOke fadeController;
    public string nextSceneName = "SpiritIconToBeContinue";

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip errorClip;      // 🔴 suara gagal
    public AudioClip successClip;    // 🟢 suara berhasil
    public AudioClip doorOpenClip;   // 🚪 suara pintu

    [Header("UI")]
    public TMP_Text interactText; // ✅ ubah ke TMP_Text

    private bool canTap = false;
    private bool doorOpened = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        if (doorTransform != null)
        {
            closedRotation = doorTransform.localRotation;
            float finalAngle = invertRotation ? -openAngle : openAngle;
            openRotation = closedRotation * Quaternion.Euler(0, finalAngle, 0);
        }

        if (greenLight != null) greenLight.SetActive(false);
        if (redLight != null) redLight.SetActive(false);

        if (interactText != null)
            interactText.gameObject.SetActive(false); // sembunyikan teks di awal

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
            bool hasExitCard = CheckExitCardInInventory();
            Debug.Log("[TapExitCard] Slot aktif berisi kartu? " + hasExitCard);

            if (hasExitCard)
            {
                if (greenLight != null) greenLight.SetActive(true);
                if (redLight != null) redLight.SetActive(false);
                StartCoroutine(ResetLights());

                if (audioSource != null && successClip != null)
                    audioSource.PlayOneShot(successClip);

                if (doorTransform != null && !doorOpened)
                    StartCoroutine(OpenDoor());
            }
            else
            {
                if (redLight != null) redLight.SetActive(true);
                if (greenLight != null) greenLight.SetActive(false);
                StartCoroutine(ResetLights());

                if (audioSource != null && errorClip != null)
                    audioSource.PlayOneShot(errorClip);
            }
        }
    }

    private IEnumerator ResetLights()
    {
        yield return new WaitForSeconds(lightResetDelay);
        if (greenLight != null) greenLight.SetActive(false);
        if (redLight != null) redLight.SetActive(false);
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
        return hasCard;
    }

    private IEnumerator OpenDoor()
    {
        doorOpened = true;
        float t = 0f;

        if (audioSource != null && doorOpenClip != null)
            audioSource.PlayOneShot(doorOpenClip);

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorTransform.localRotation = Quaternion.Slerp(closedRotation, openRotation, t);
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        if (fadeController != null)
            fadeController.FadeToNextScene(nextSceneName);
        else
            Debug.LogWarning("[TapExitCard] ⚠️ FadeController belum di-assign.");

        yield return new WaitForSeconds(3f);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorTransform.localRotation = Quaternion.Slerp(openRotation, closedRotation, t);
            yield return null;
        }

        doorOpened = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTap = true;
            if (interactText != null)
            {
                interactText.text = "Press E to open Exit Door";
                interactText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTap = false;
            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }
    }
}
