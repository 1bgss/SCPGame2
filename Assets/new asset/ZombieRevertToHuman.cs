using UnityEngine;

public class ZombieRevertToHuman : MonoBehaviour
{
    [Header("=== TEKAN TAB ATAU T UNTUK BALIK KE HUMAN ===")]
    public KeyCode revertKey1 = KeyCode.Tab;
    public KeyCode revertKey2 = KeyCode.T;

    [Header("=== ISI SEMUA INI DI INSPECTOR ===")]
    public GameObject playerObject;
    public GameObject zombieObject;
    public Camera fppCamera;
    public Camera tppCamera;
    public PlayerMovement playerScript;
    public THC1_ctrl zombieScript;

    [Header("=== UI (OPTIONAL) ===")]
    public GameObject humanUI;
    public GameObject zombieUI;
    public GameObject inventoryPanel;
    public InventoryToggle inventoryToggle;
    public InventoryInputManager inventoryInputManager;

    [Header("=== HEIGHT FIX ===")]
    public float headHeightOffset = 1.6f;   // tinggi manusia (kepala)

    void Update()
    {
        if (Input.GetKeyDown(revertKey1) || Input.GetKeyDown(revertKey2))
        {
            RevertToHuman();
        }
    }

    void RevertToHuman()
    {
        Debug.Log("\n════════ REVERTING TO HUMAN ════════");

        // ========== 1. SYNC POSISI KE TITIK KEPALA ZOMBIE ==========
        if (playerObject != null && zombieObject != null)
        {
            Vector3 syncPos = zombieObject.transform.position;
            syncPos.y += headHeightOffset; // spawn human sedikit lebih tinggi
            playerObject.transform.SetPositionAndRotation(syncPos, zombieObject.transform.rotation);

            Debug.Log("📌 Sync Player = Posisi kepala zombie");
        }

        // ========== 2. TAMPILKAN PLAYER ==========
        playerObject?.SetActive(true);
        fppCamera?.gameObject.SetActive(true);
        if (fppCamera != null) fppCamera.enabled = true;
        if (playerScript != null) playerScript.enabled = true;

        // ========== 3. SEMBUNYIKAN ZOMBIE ==========
        zombieObject?.SetActive(false);
        tppCamera?.gameObject.SetActive(false);
        if (tppCamera != null) tppCamera.enabled = false;
        if (zombieScript != null) zombieScript.enabled = false;

        // ========== 4. UI ==========
        humanUI?.SetActive(true);
        if (zombieUI != null) zombieUI.SetActive(false);
        inventoryPanel?.SetActive(true);
        if (inventoryToggle != null) inventoryToggle.enabled = true;
        if (inventoryInputManager != null) inventoryInputManager.enabled = true;

        // ========== 5. CURSOR ==========
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("✓ REVERTED TO HUMAN MODE");
        Debug.Log("════════════════════════════════════\n");
    }
}
