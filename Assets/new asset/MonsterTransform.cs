using UnityEngine;

public class MonsterTransform : MonoBehaviour
{
    [Header("=== TEKAN TAB ATAU T UNTUK TRANSFORM ===")]
    public KeyCode transformKey = KeyCode.Tab;
    public KeyCode altTransformKey = KeyCode.T; // Backup key
    
    [Header("=== ISI SEMUA INI DI INSPECTOR ===")]
    public GameObject playerModel;      // FPP hands/weapon
    public GameObject zombieModel;      // Zombie full body
    public Camera fppCamera;            // Main Camera FPP
    public Camera tppCamera;            // TPP Camera untuk zombie
    public PlayerMovement playerScript; // Script player movement
    public THC1_ctrl zombieScript;      // Script zombie controller
    
    [Header("=== UI (OPTIONAL) ===")]
    public GameObject humanUI;          // UI untuk human mode
    public GameObject zombieUI;         // UI untuk zombie mode
    public GameObject inventoryPanel;   // Inventory panel (hide saat zombie)
    
    [Header("=== INVENTORY SCRIPTS (DISABLE SAAT ZOMBIE) ===")]
    public InventoryToggle inventoryToggle;
    public InventoryInputManager inventoryInputManager;

    [Header("=== DEFAULT CAMERA TRANSFORMS ===")]
    public Transform fppDefaultTransform; // Posisi & Rotasi kamera saat Human
    public Transform tppDefaultTransform; // Posisi & Rotasi kamera saat Zombie
    
    private bool isZombie = false;
    
    void Start()
    {
        Debug.Log("╔════════════════════════════════════════╗");
        Debug.Log("║  MONSTER TRANSFORM - TEKAN TAB         ║");
        Debug.Log("╚════════════════════════════════════════╝");
        
        // MULAI DARI HUMAN MODE
        ShowHuman();
    }
    
    void Update()
    {
        // DETEKSI TAB ATAU T
        if (Input.GetKeyDown(transformKey) || Input.GetKeyDown(altTransformKey))
        {
            Debug.Log("═══════════════════════════════════════");
            Debug.Log($"TRANSFORM KEY PRESSED! Current: {(isZombie ? "ZOMBIE" : "HUMAN")}");
            
            if (isZombie)
                ShowHuman();
            else
                ShowZombie();
        }
    }
    
    void ShowHuman()
    {
        Debug.Log("════════ SHOWING HUMAN ════════");
        
        isZombie = false;
        
        // TAMPILKAN PLAYER
        if (playerModel != null)
            playerModel.SetActive(true);
        
        // SEMBUNYIKAN ZOMBIE
        if (zombieModel != null)
            zombieModel.SetActive(false);
        
        // AKTIFKAN FPP CAMERA
        if (fppCamera != null)
        {
            fppCamera.gameObject.SetActive(true);
            fppCamera.enabled = true;

            // RESET POSISI & ROTASI
            if (fppDefaultTransform != null)
            {
                fppCamera.transform.position = fppDefaultTransform.position;
                fppCamera.transform.rotation = fppDefaultTransform.rotation;
            }
        }
        
        // MATIKAN TPP CAMERA
        if (tppCamera != null)
        {
            tppCamera.enabled = false;
            tppCamera.gameObject.SetActive(false);
        }
        
        // AKTIFKAN PLAYER SCRIPT
        if (playerScript != null)
            playerScript.enabled = true;
        
        // MATIKAN ZOMBIE SCRIPT
        if (zombieScript != null)
            zombieScript.enabled = false;
        
        // TAMPILKAN UI HUMAN
        if (humanUI != null)
            humanUI.SetActive(true);
        
        // SEMBUNYIKAN UI ZOMBIE
        if (zombieUI != null)
            zombieUI.SetActive(false);
        
        // TAMPILKAN INVENTORY
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);
        
        // AKTIFKAN INVENTORY SCRIPTS
        if (inventoryToggle != null)
            inventoryToggle.enabled = true;
        
        if (inventoryInputManager != null)
            inventoryInputManager.enabled = true;
        
        Debug.Log("════════ HUMAN MODE AKTIF ════════");
    }
    
    void ShowZombie()
    {
        Debug.Log("════════ SHOWING ZOMBIE ════════");
        
        isZombie = true;
        
        // SEMBUNYIKAN PLAYER
        if (playerModel != null)
            playerModel.SetActive(false);
        
        // TAMPILKAN ZOMBIE
        if (zombieModel != null)
            zombieModel.SetActive(true);
        
        // MATIKAN FPP CAMERA
        if (fppCamera != null)
        {
            fppCamera.enabled = false;
            fppCamera.gameObject.SetActive(false);
        }
        
        // AKTIFKAN TPP CAMERA
        if (tppCamera != null)
        {
            tppCamera.gameObject.SetActive(true);
            tppCamera.enabled = true;

            // RESET POSISI & ROTASI
            if (tppDefaultTransform != null)
            {
                tppCamera.transform.position = tppDefaultTransform.position;
                tppCamera.transform.rotation = tppDefaultTransform.rotation;
            }
        }
        
        // MATIKAN PLAYER SCRIPT
        if (playerScript != null)
            playerScript.enabled = false;
        
        // AKTIFKAN ZOMBIE SCRIPT
        if (zombieScript != null)
            zombieScript.enabled = true;
        
        // SEMBUNYIKAN UI HUMAN
        if (humanUI != null)
            humanUI.SetActive(false);
        
        // TAMPILKAN UI ZOMBIE
        if (zombieUI != null)
            zombieUI.SetActive(true);
        
        // SEMBUNYIKAN INVENTORY
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        
        // MATIKAN INVENTORY SCRIPTS
        if (inventoryToggle != null)
            inventoryToggle.enabled = false;
        
        if (inventoryInputManager != null)
            inventoryInputManager.enabled = false;
        
        Debug.Log("════════ ZOMBIE MODE AKTIF ════════");
    }
    
    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 300, 100), "MONSTER TRANSFORM");
        GUI.Label(new Rect(20, 35, 280, 20), $"Mode: {(isZombie ? "ZOMBIE" : "HUMAN")}");
        GUI.Label(new Rect(20, 55, 280, 20), $"Press [{transformKey}] or [{altTransformKey}] to toggle");
        GUI.Label(new Rect(20, 75, 280, 20), "Check Console for details!");
    }
}
