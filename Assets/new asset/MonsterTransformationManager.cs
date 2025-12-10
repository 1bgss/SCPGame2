using UnityEngine;
using UnityEngine.UI;

/*
═══════════════════════════════════════════════════════════════════════════════
            MONSTER TRANSFORMATION - POSITION SYNC FIXED
═══════════════════════════════════════════════════════════════════════════════
FIXES:
✓ Player dan Monster posisi sinkron saat transform
✓ Monster gerak = Player ikut gerak (posisi sinkron realtime)
✓ Transform balik ke human tetap di posisi terakhir
✓ Tidak ada "teleport" saat transform
═══════════════════════════════════════════════════════════════════════════════
*/

public class MonsterTransformationManager : MonoBehaviour
{
    [Header("=== TRANSFORMATION SETTINGS ===")]
    public KeyCode transformKey = KeyCode.T;
    [Tooltip("Set to 0 for unlimited duration (manual toggle only)")]
    public float transformDuration = 0f;
    public bool isMonster = false;
    
    [Header("=== DEBUG MODE ===")]
    public bool showDebugInfo = true;
    
    [Header("=== GAMEOBJECT REFERENCES ===")]
    public GameObject humanModel;
    public GameObject monsterModel;
    public THC1_ctrl monsterController;
    
    // TAMBAHAN BARU: Reference ke Player GameObject
    [Header("=== PLAYER REFERENCE (PENTING!) ===")]
    [Tooltip("Drag Player GameObject (yang punya CharacterController) ke sini")]
    public GameObject playerGameObject;
    private CharacterController playerCharacterController;
    private CharacterController monsterCharacterController;
    
    [Header("=== 2 CAMERA SYSTEM ===")]
    public Camera humanCamera;
    public Camera monsterCamera;
    public float tppDistance = 4f;
    public float tppHeight = 2.5f;
    public float cameraSmoothSpeed = 10f;
    
    [Header("=== HEALTH SETTINGS ===")]
    public float humanMaxHealth = 100f;
    public float monsterMaxHealth = 200f;
    private float currentHumanHealth = 100f;
    private float currentMonsterHealth = 200f;
    
    [Header("=== STAMINA SETTINGS ===")]
    public PlayerStamina playerStamina;
    private float humanMaxStamina = 5f;
    public float monsterMaxStamina = 200f;
    public float monsterStaminaDrainRun = 10f;
    public float monsterStaminaDrainAttack = 10f;
    private float currentMonsterStamina = 200f;
    
    [Header("=== COMBAT SETTINGS ===")]
    public float monsterDamageNormal = 10f;
    public float monsterDamageUltimate = 20f;
    
    [Header("=== UI REFERENCES ===")]
    public GameObject humanUI;
    public GameObject monsterUI;
    public GameObject inventoryPanel;
    public Slider monsterHealthSlider;
    public Text monsterHealthText;
    public Slider monsterStaminaSlider;
    public Text monsterStaminaText;
    public Text transformTimerText;
    public Text transformHintText;
    
    [Header("=== AUDIO SETTINGS ===")]
    public AudioSource monsterAudioSource;
    public AudioClip monsterWalkSound;
    public AudioClip monsterRunSound;
    public AudioClip monsterAttackSound;
    public AudioClip monsterRoarSound;
    public AudioClip monsterHitSound;
    public AudioClip monsterDeathSound;
    public AudioClip transformSound;
    public AudioClip revertSound;
    
    [Header("=== PLAYER SCRIPTS ===")]
    public PlayerMovement playerMovement;
    public InventoryToggle inventoryToggle;
    public InventoryInputManager inventoryInputManager;
    
    private float transformTimer = 0f;
    private Animator monsterAnimator;
    private float footstepTimer = 0f;
    private bool isDead = false;
    private bool isTransforming = false;
    
    void Start()
    {
        if (monsterModel != null)
        {
            monsterAnimator = monsterModel.GetComponent<Animator>();
            monsterCharacterController = monsterModel.GetComponent<CharacterController>();
        }
        
        // Auto-find player GameObject jika belum di-set
        if (playerGameObject == null)
        {
            // Cari object dengan tag "Player"
            playerGameObject = GameObject.FindGameObjectWithTag("Player");
            
            // Atau cari PlayerMovement script di scene
            if (playerGameObject == null && playerMovement != null)
            {
                playerGameObject = playerMovement.gameObject;
            }
        }
        
        if (playerGameObject != null)
        {
            playerCharacterController = playerGameObject.GetComponent<CharacterController>();
            Debug.Log($"✅ Player GameObject found: {playerGameObject.name}");
        }
        else
        {
            Debug.LogError("❌ Player GameObject NOT FOUND! Please assign it in Inspector!");
        }
        
        if (playerStamina != null)
            humanMaxStamina = playerStamina.maxStamina;
        
        currentHumanHealth = humanMaxHealth;
        currentMonsterHealth = monsterMaxHealth;
        currentMonsterStamina = monsterMaxStamina;
        
        SetHumanMode();
        
        Debug.Log("╔════════════════════════════════════════════════════════╗");
        Debug.Log("║     🐺 MONSTER TRANSFORMATION SYSTEM READY 🐺          ║");
        Debug.Log("╚════════════════════════════════════════════════════════╝");
        Debug.Log($"Transform Key: [{transformKey}]");
        
        if (transformDuration > 0)
        {
            Debug.Log($"Mode: AUTO-REVERT after {transformDuration} seconds");
            Debug.Log($"  ⚠️ You can press [{transformKey}] anytime to revert manually!");
        }
        else
        {
            Debug.Log($"Mode: MANUAL TOGGLE (no timer)");
            Debug.Log($"  ✓ Press [{transformKey}] to toggle between Human ⟷ Monster");
        }
        
        Debug.Log("════════════════════════════════════════════════════════");
    }
    
    void Update()
    {
        HandleTransformationInput();
        
        if (isMonster)
        {
            UpdateMonsterMode();
            
            if (transformDuration > 0)
            {
                UpdateTransformTimer();
            }
            
            UpdateTPPCamera();
            
            // CRITICAL: SINKRONKAN POSISI PLAYER KE MONSTER SETIAP FRAME!
            SyncPlayerToMonster();
        }
        
        UpdateHintText();
    }
    
    // ==========================================
    // POSITION SYNC SYSTEM - SOLUSI UTAMA!
    // ==========================================
    
    void SyncPlayerToMonster()
    {
        if (playerGameObject == null || monsterModel == null) return;
        
        // Sinkronkan posisi Player ke Monster
        // Jadi pas Monster gerak, Player ikut gerak di posisi yang sama
        playerGameObject.transform.position = monsterModel.transform.position;
        playerGameObject.transform.rotation = monsterModel.transform.rotation;
    }
    
    void SyncMonsterToPlayer()
    {
        if (playerGameObject == null || monsterModel == null) return;
        
        // Sinkronkan posisi Monster ke Player
        // Dipanggil saat transform jadi monster
        monsterModel.transform.position = playerGameObject.transform.position;
        monsterModel.transform.rotation = playerGameObject.transform.rotation;
    }
    
    // ==========================================
    // TRANSFORMATION INPUT HANDLER
    // ==========================================
    
    void HandleTransformationInput()
    {
        if (Input.GetKeyDown(transformKey))
        {
            if (showDebugInfo)
            {
                Debug.Log("╔════════════════════════════════════════════════════════╗");
                Debug.Log("║ [T KEY DETECTED] - PRIORITY CHECK                      ║");
                Debug.Log($"║ isMonster: {isMonster,-5} isDead: {isDead,-5} isTransforming: {isTransforming,-5}║");
                Debug.Log($"║ Script enabled: {enabled,-5}                                  ║");
                Debug.Log("╚════════════════════════════════════════════════════════╝");
            }
            
            if (!isDead && !isTransforming)
            {
                if (showDebugInfo) Debug.Log($"[✓] Conditions met - Calling ToggleTransformation()");
                ToggleTransformation();
            }
            else
            {
                if (showDebugInfo) Debug.Log($"[✗] Blocked: isDead={isDead} OR isTransforming={isTransforming}");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (showDebugInfo)
            {
                Debug.Log("╔════════════════════════════════════════════════════════╗");
                Debug.Log("║ [K KEY] EMERGENCY REVERT TO HUMAN!                     ║");
                Debug.Log("╚════════════════════════════════════════════════════════╝");
            }
            
            if (isMonster)
            {
                ForceTransformToHuman();
            }
        }
    }
    
    void ToggleTransformation()
    {
        if (showDebugInfo)
        {
            Debug.Log($"╔════════════════════════════════════════════════════════╗");
            Debug.Log($"║ [TOGGLE TRANSFORMATION]                                ║");
            Debug.Log($"║ Current State: {(isMonster ? "MONSTER" : "HUMAN"),-35} ║");
            Debug.Log($"║ Next Action: {(isMonster ? "→ HUMAN" : "→ MONSTER"),-37} ║");
            Debug.Log($"╚════════════════════════════════════════════════════════╝");
        }
        
        if (!isMonster)
        {
            if (showDebugInfo) Debug.Log("→ Calling StartTransformation()");
            StartTransformation();
        }
        else
        {
            if (showDebugInfo) Debug.Log("→ Calling EndTransformation()");
            EndTransformation();
        }
    }
    
    void StartTransformation()
    {
        isMonster = true;
        isDead = false;
        isTransforming = true;
        
        if (transformDuration > 0)
        {
            transformTimer = transformDuration;
        }
        
        Debug.Log("🐺 TRANSFORMING INTO MONSTER!");
        
        // SYNC POSISI SEBELUM TRANSFORM
        SyncMonsterToPlayer();
        
        ConvertHealthToMonster();
        currentMonsterStamina = monsterMaxStamina;
        
        SetMonsterMode();
        
        if (monsterAudioSource != null)
        {
            if (transformSound != null)
            {
                monsterAudioSource.PlayOneShot(transformSound);
            }
            else if (monsterRoarSound != null)
            {
                monsterAudioSource.PlayOneShot(monsterRoarSound);
            }
        }
        
        Invoke("FinishTransformation", 0.5f);
    }
    
    void FinishTransformation()
    {
        isTransforming = false;
    }
    
    void EndTransformation()
    {
        if (showDebugInfo)
        {
            Debug.Log("╔════════════════════════════════════════════════════════╗");
            Debug.Log("║          🔄 REVERTING TO HUMAN                         ║");
            Debug.Log("╚════════════════════════════════════════════════════════╝");
        }
        
        // SYNC POSISI PLAYER KE MONSTER SEBELUM REVERT
        // Ini penting! Supaya player muncul di posisi terakhir monster
        if (playerGameObject != null && monsterModel != null)
        {
            playerGameObject.transform.position = monsterModel.transform.position;
            playerGameObject.transform.rotation = monsterModel.transform.rotation;
            
            if (showDebugInfo)
            {
                Debug.Log($"[SYNC] Player position set to: {monsterModel.transform.position}");
            }
        }
        
        bool wasMonster = isMonster;
        isMonster = false;
        isDead = false;
        isTransforming = true;
        
        if (showDebugInfo)
        {
            Debug.Log($"[FLAGS] isMonster: {wasMonster} → {isMonster}");
            Debug.Log($"[FLAGS] isDead: {isDead}");
            Debug.Log($"[FLAGS] isTransforming: {isTransforming}");
        }
        
        ConvertHealthToHuman();
        SetHumanMode();
        transformTimer = 0f;
        
        if (monsterAudioSource != null && revertSound != null)
        {
            monsterAudioSource.PlayOneShot(revertSound);
        }
        
        Invoke("FinishTransformation", 0.5f);
        
        if (showDebugInfo)
        {
            Debug.Log("╔════════════════════════════════════════════════════════╗");
            Debug.Log("║          ✓ REVERT TO HUMAN COMPLETE                    ║");
            Debug.Log("╚════════════════════════════════════════════════════════╝");
        }
    }
    
    void UpdateTransformTimer()
    {
        if (transformTimer > 0)
        {
            transformTimer -= Time.deltaTime;
            
            if (transformTimerText != null)
            {
                transformTimerText.text = "Transform: " + Mathf.CeilToInt(transformTimer) + "s";
            }
            
            if (transformTimer <= 5f && transformTimer > 4f)
            {
                if (showDebugInfo) Debug.Log("⚠️ Transform ending in 5 seconds! (Press T to revert manually)");
            }
            
            if (transformTimer <= 0)
            {
                if (showDebugInfo)
                {
                    Debug.Log("╔════════════════════════════════════════════════════════╗");
                    Debug.Log("║          ⏰ TIMER EXPIRED - AUTO REVERT                ║");
                    Debug.Log("╚════════════════════════════════════════════════════════╝");
                }
                EndTransformation();
            }
        }
    }
    
    void UpdateHintText()
    {
        if (transformHintText == null) return;
        
        if (isDead)
        {
            transformHintText.text = "";
        }
        else if (isMonster)
        {
            if (transformDuration > 0 && transformTimer > 0)
            {
                transformHintText.text = $"Press [{transformKey}] to Revert to Human (or wait {Mathf.CeilToInt(transformTimer)}s)";
            }
            else
            {
                transformHintText.text = $"Press [{transformKey}] to Revert to Human";
            }
        }
        else
        {
            transformHintText.text = $"Press [{transformKey}] to Transform to Monster";
        }
    }
    
    void ConvertHealthToMonster()
    {
        float healthPercentage = currentHumanHealth / humanMaxHealth;
        currentMonsterHealth = Mathf.Max(healthPercentage * monsterMaxHealth, monsterMaxHealth * 0.5f);
        
        Debug.Log($"💚 Health: {currentHumanHealth} → {currentMonsterHealth}");
    }
    
    void ConvertHealthToHuman()
    {
        float healthPercentage = currentMonsterHealth / monsterMaxHealth;
        currentHumanHealth = Mathf.Max(healthPercentage * humanMaxHealth, humanMaxHealth * 0.1f);
        
        Debug.Log($"💚 Health: {currentMonsterHealth} → {currentHumanHealth}");
    }
    
    void SetHumanMode()
    {
        if (showDebugInfo)
        {
            Debug.Log("╔════════════════════════════════════════════════════════╗");
            Debug.Log("║             ACTIVATING HUMAN MODE                      ║");
            Debug.Log("╚════════════════════════════════════════════════════════╝");
        }
        
        if (humanModel != null)
        {
            humanModel.SetActive(true);
            if (showDebugInfo) Debug.Log("✅ Human Model → ACTIVE");
        }
        
        if (monsterModel != null)
        {
            monsterModel.SetActive(false);
            if (showDebugInfo) Debug.Log("❌ Monster Model → HIDDEN");
        }
        
        if (humanCamera != null)
        {
            humanCamera.gameObject.SetActive(true);
            humanCamera.enabled = true;
            humanCamera.tag = "MainCamera";
            if (showDebugInfo) Debug.Log("✅ FPP Camera → ACTIVE");
        }
        
        if (monsterCamera != null)
        {
            monsterCamera.gameObject.SetActive(false);
            monsterCamera.enabled = false;
            monsterCamera.tag = "Untagged";
            if (showDebugInfo) Debug.Log("❌ TPP Camera → INACTIVE");
        }
        
        if (humanUI != null) humanUI.SetActive(true);
        if (monsterUI != null) monsterUI.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
        
        if (playerMovement != null) 
        {
            playerMovement.enabled = true;
            if (showDebugInfo) Debug.Log("✅ Player Movement → ENABLED");
        }
        if (inventoryToggle != null) inventoryToggle.enabled = true;
        if (inventoryInputManager != null) inventoryInputManager.enabled = true;
        
        if (monsterController != null)
        {
            monsterController.enabled = false;
            if (showDebugInfo) Debug.Log("❌ Monster Controller → DISABLED");
        }
        
        if (playerStamina != null)
        {
            playerStamina.maxStamina = humanMaxStamina;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (showDebugInfo)
        {
            Debug.Log("╔════════════════════════════════════════════════════════╗");
            Debug.Log("║             ✓ HUMAN MODE ACTIVE                        ║");
            Debug.Log("╚════════════════════════════════════════════════════════╝");
        }
    }
    
    void SetMonsterMode()
    {
        Debug.Log("=== SETTING MONSTER MODE ===");
        
        if (humanModel != null)
        {
            humanModel.SetActive(false);
            Debug.Log("❌ Human Model OFF (HIDDEN)");
        }
        
        if (monsterModel != null)
        {
            monsterModel.SetActive(true);
            Debug.Log("✅ Monster Model ON");
        }
        
        if (humanCamera != null)
        {
            humanCamera.gameObject.SetActive(false);
            humanCamera.enabled = false;
            humanCamera.tag = "Untagged";
            Debug.Log("❌ FPP Camera OFF");
        }
        
        if (monsterCamera != null)
        {
            monsterCamera.gameObject.SetActive(true);
            monsterCamera.enabled = true;
            monsterCamera.tag = "MainCamera";
            Debug.Log("✅ TPP Camera ON");
        }
        
        if (humanUI != null) humanUI.SetActive(false);
        if (monsterUI != null) monsterUI.SetActive(true);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        
        if (playerMovement != null) 
        {
            playerMovement.enabled = false;
            Debug.Log("❌ Player Movement OFF");
        }
        if (inventoryToggle != null) inventoryToggle.enabled = false;
        if (inventoryInputManager != null) inventoryInputManager.enabled = false;
        
        if (monsterController != null)
        {
            monsterController.enabled = true;
            Debug.Log("✅ Monster Controller ON");
            
            if (monsterAnimator != null)
            {
                monsterAnimator.SetInteger("battle", 3);
                monsterAnimator.SetInteger("moving", 0);
                Debug.Log("✅ Monster set to SLEEP MODE (battle=3, moving=0)");
                Debug.Log("   Press 1 or 2 to wake up to Combat Mode");
            }
        }
        
        UpdateMonsterUI();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("✅ === MONSTER MODE ACTIVE ===");
        Debug.Log("   Current Controls: Press 1/2 for Combat, 3 for Crawl, 4 for Sleep");
    }
    
    void UpdateTPPCamera()
    {
        if (monsterCamera == null || monsterModel == null) return;
        
        Vector3 targetPosition = monsterModel.transform.position
            - monsterModel.transform.forward * tppDistance
            + Vector3.up * tppHeight;
        
        monsterCamera.transform.position = Vector3.Lerp(
            monsterCamera.transform.position,
            targetPosition,
            Time.deltaTime * cameraSmoothSpeed
        );
        
        Vector3 lookTarget = monsterModel.transform.position + Vector3.up * 1.5f;
        monsterCamera.transform.LookAt(lookTarget);
    }
    
    void UpdateMonsterMode()
    {
        if (monsterController != null)
        {
            bool isRunning = monsterController.IsRunning();
            
            if (isRunning && currentMonsterStamina > 0)
            {
                currentMonsterStamina -= monsterStaminaDrainRun * Time.deltaTime;
                if (currentMonsterStamina < 0) currentMonsterStamina = 0;
            }
            else if (!isRunning && currentMonsterStamina < monsterMaxStamina)
            {
                currentMonsterStamina += 5f * Time.deltaTime;
                if (currentMonsterStamina > monsterMaxStamina)
                    currentMonsterStamina = monsterMaxStamina;
            }
        }
        
        HandleMonsterFootsteps();
        UpdateMonsterUI();
        
        if (currentMonsterHealth <= 0 && !isDead)
        {
            MonsterDeath();
        }
    }
    
    void UpdateMonsterUI()
    {
        if (monsterHealthSlider != null)
        {
            monsterHealthSlider.maxValue = monsterMaxHealth;
            monsterHealthSlider.value = currentMonsterHealth;
        }
        if (monsterHealthText != null)
        {
            monsterHealthText.text = $"HP: {Mathf.CeilToInt(currentMonsterHealth)}/{monsterMaxHealth}";
        }
        
        if (monsterStaminaSlider != null)
        {
            monsterStaminaSlider.maxValue = monsterMaxStamina;
            monsterStaminaSlider.value = currentMonsterStamina;
        }
        if (monsterStaminaText != null)
        {
            monsterStaminaText.text = $"STAMINA: {Mathf.CeilToInt(currentMonsterStamina)}/{monsterMaxStamina}";
        }
        
        if (transformTimerText != null)
        {
            if (transformDuration > 0 && transformTimer > 0)
            {
                transformTimerText.text = "Transform: " + Mathf.CeilToInt(transformTimer) + "s";
            }
            else if (transformDuration <= 0)
            {
                transformTimerText.text = "Transform: ∞";
            }
        }
    }
    
    void HandleMonsterFootsteps()
    {
        if (monsterController == null || monsterAudioSource == null) return;
        
        bool isMoving = monsterController.IsMoving();
        bool isRunning = monsterController.IsRunning();
        
        if (!isMoving)
        {
            footstepTimer = 0;
            return;
        }
        
        footstepTimer += Time.deltaTime;
        float stepRate = isRunning ? 0.3f : 0.5f;
        
        if (footstepTimer >= stepRate)
        {
            AudioClip clip = isRunning ? monsterRunSound : monsterWalkSound;
            if (clip != null && !monsterAudioSource.isPlaying)
            {
                monsterAudioSource.PlayOneShot(clip);
            }
            footstepTimer = 0f;
        }
    }
    
    public void PlayMonsterSound(string soundType)
    {
        if (monsterAudioSource == null) return;
        
        AudioClip clip = null;
        switch (soundType.ToLower())
        {
            case "attack": clip = monsterAttackSound; break;
            case "roar": clip = monsterRoarSound; break;
            case "hit": clip = monsterHitSound; break;
            case "death": clip = monsterDeathSound; break;
            case "transform": clip = transformSound; break;
            case "revert": clip = revertSound; break;
        }
        
        if (clip != null)
        {
            monsterAudioSource.PlayOneShot(clip);
        }
    }
    
    public void MonsterAttack(bool isUltimate = false)
    {
        if (!isMonster || isDead) return;
        
        if (currentMonsterStamina < monsterStaminaDrainAttack)
        {
            Debug.Log("❌ Not enough stamina!");
            return;
        }
        
        currentMonsterStamina -= monsterStaminaDrainAttack;
        PlayMonsterSound("attack");
        
        float damage = isUltimate ? monsterDamageUltimate : monsterDamageNormal;
        Debug.Log($"💥 Monster attacks! Damage: {damage}");
    }
    
    public void MonsterTakeDamage(float damage)
    {
        if (!isMonster || isDead) return;
        
        currentMonsterHealth -= damage;
        if (currentMonsterHealth < 0) currentMonsterHealth = 0;
        
        PlayMonsterSound("hit");
        
        if (monsterAnimator != null)
        {
            monsterAnimator.SetInteger("moving", 10);
        }
        
        Debug.Log($"💔 Took {damage} damage! HP: {currentMonsterHealth}");
        
        if (currentMonsterHealth <= 0)
        {
            MonsterDeath();
        }
    }
    
    void MonsterDeath()
    {
        isDead = true;
        Debug.Log("💀 MONSTER DIED!");
        
        PlayMonsterSound("death");
        
        if (monsterAnimator != null)
        {
            int deathAnim = Random.Range(0, 2) == 0 ? 12 : 13;
            monsterAnimator.SetInteger("moving", deathAnim);
        }
        
        if (monsterController != null)
            monsterController.enabled = false;
        
        Invoke("RevertAfterDeath", 3f);
    }
    
    void RevertAfterDeath()
    {
        currentHumanHealth = 10f;
        EndTransformation();
    }
    
    public bool IsMonsterMode()
    {
        return isMonster;
    }
    
    public float GetMonsterStamina()
    {
        return currentMonsterStamina;
    }
    
    public bool HasMonsterStamina()
    {
        return currentMonsterStamina > 0;
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    public void ForceTransformToMonster()
    {
        if (!isMonster)
        {
            StartTransformation();
        }
    }
    
    public void ForceTransformToHuman()
    {
        if (isMonster)
        {
            EndTransformation();
        }
    }
}

/*
═══════════════════════════════════════════════════════════════════════════════
                        CARA SETUP DI INSPECTOR
═══════════════════════════════════════════════════════════════════════════════

1. DRAG REFERENCES:
   ✓ Player GameObject → ke field "Player Game Object" (PENTING!)
   ✓ Monster Model → ke field "Monster Model"
   ✓ Human Model → ke field "Human Model"

2. HIERARCHY STRUCTURE:
   - Player (CharacterController)
     └─ [Human Model/Weapon/FPP Hands]
   
   - hor_mon_1.1 (CharacterController + THC1_ctrl)
     └─ [Monster 3D Model]

3. CARA KERJA:
   ✓ Saat jadi Monster → Player position sync ke Monster realtime
   ✓ Saat Monster gerak → Player ikut gerak (invisible, posisi sama)
   ✓ Saat revert ke Human → Player muncul di posisi terakhir Monster
   ✓ TIDAK ADA TELEPORT! Semua smooth!

═══════════════════════════════════════════════════════════════════════════════
*/