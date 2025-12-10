using UnityEngine;
using UnityEngine.UI;

/*
═══════════════════════════════════════════════════════════════════════════════
            MONSTER TRANSFORMATION - FIXED VERSION
═══════════════════════════════════════════════════════════════════════════════
FIXES:
- Monster start in SLEEP mode (not combat)
- T key properly toggles back to human
- Monster hidden when reverting to human
- Proper camera switching
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
    public bool showDebugInfo = true; // Toggle debug messages
    
    [Header("=== GAMEOBJECT REFERENCES ===")]
    public GameObject humanModel;           // Player FPP hands/weapon
    public GameObject monsterModel;         // Zombie full body
    public THC1_ctrl monsterController;     // Zombie controller script
    
    [Header("=== 2 CAMERA SYSTEM ===")]
    public Camera humanCamera;              // Main Camera (FPP)
    public Camera monsterCamera;            // Main Camera TPP
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
    
    // Private variables
    private float transformTimer = 0f;
    private Animator monsterAnimator;
    private float footstepTimer = 0f;
    private bool isDead = false;
    private bool isTransforming = false;
    
    void Start()
    {
        // Get components
        if (monsterModel != null)
        {
            monsterAnimator = monsterModel.GetComponent<Animator>();
        }
        
        if (playerStamina != null)
            humanMaxStamina = playerStamina.maxStamina;
        
        // Initialize health
        currentHumanHealth = humanMaxHealth;
        currentMonsterHealth = monsterMaxHealth;
        currentMonsterStamina = monsterMaxStamina;
        
        // Setup initial state - HUMAN MODE
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
        // CRITICAL: Always check for transformation key FIRST, regardless of mode
        // This runs BEFORE any other script can consume the input
        HandleTransformationInput();
        
        // Update systems based on current mode
        if (isMonster)
        {
            UpdateMonsterMode();
            
            if (transformDuration > 0)
            {
                UpdateTransformTimer();
            }
            
            UpdateTPPCamera();
        }
        
        UpdateHintText();
    }
    
    void HandleTransformationInput()
    {
        // Check EVERY frame for transformation key
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
        
        // BACKUP KEY - K for emergency revert
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
    
    // ==========================================
    // TRANSFORMATION SYSTEM - TOGGLE
    // ==========================================
    
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
        
        ConvertHealthToMonster();
        currentMonsterStamina = monsterMaxStamina;
        
        SetMonsterMode();
        
        // Play transform sound
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
        
        // Allow input setelah transform selesai
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
        
        // Set flags IMMEDIATELY
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
        
        // Play revert sound
        if (monsterAudioSource != null && revertSound != null)
        {
            monsterAudioSource.PlayOneShot(revertSound);
        }
        
        // Allow input setelah revert selesai
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
            
            // Warning saat waktunya hampir habis
            if (transformTimer <= 5f && transformTimer > 4f)
            {
                if (showDebugInfo) Debug.Log("⚠️ Transform ending in 5 seconds! (Press T to revert manually)");
            }
            
            // AUTO REVERT saat timer habis
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
    
    // ==========================================
    // HEALTH CONVERSION
    // ==========================================
    
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
    
    // ==========================================
    // MODE SWITCHING - FIXED VERSION
    // ==========================================
    
    void SetHumanMode()
    {
        if (showDebugInfo)
        {
            Debug.Log("╔════════════════════════════════════════════════════════╗");
            Debug.Log("║             ACTIVATING HUMAN MODE                      ║");
            Debug.Log("╚════════════════════════════════════════════════════════╝");
        }
        
        // ========== MODELS ==========
        if (humanModel != null)
        {
            humanModel.SetActive(true);
            if (showDebugInfo) Debug.Log("✅ Human Model → ACTIVE");
        }
        else if (showDebugInfo) Debug.LogWarning("⚠️ Human Model reference is NULL!");
        
        if (monsterModel != null)
        {
            monsterModel.SetActive(false);
            if (showDebugInfo) Debug.Log("❌ Monster Model → HIDDEN");
        }
        else if (showDebugInfo) Debug.LogWarning("⚠️ Monster Model reference is NULL!");
        
        // ========== CAMERAS ==========
        if (humanCamera != null)
        {
            humanCamera.gameObject.SetActive(true);
            humanCamera.enabled = true;
            humanCamera.tag = "MainCamera";
            if (showDebugInfo) Debug.Log("✅ FPP Camera → ACTIVE");
        }
        else if (showDebugInfo) Debug.LogWarning("⚠️ Human Camera reference is NULL!");
        
        if (monsterCamera != null)
        {
            monsterCamera.gameObject.SetActive(false);
            monsterCamera.enabled = false;
            monsterCamera.tag = "Untagged";
            if (showDebugInfo) Debug.Log("❌ TPP Camera → INACTIVE");
        }
        else if (showDebugInfo) Debug.LogWarning("⚠️ Monster Camera reference is NULL!");
        
        // ========== UI ==========
        if (humanUI != null) humanUI.SetActive(true);
        if (monsterUI != null) monsterUI.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
        
        // ========== PLAYER SCRIPTS ==========
        if (playerMovement != null) 
        {
            playerMovement.enabled = true;
            if (showDebugInfo) Debug.Log("✅ Player Movement → ENABLED");
        }
        if (inventoryToggle != null) inventoryToggle.enabled = true;
        if (inventoryInputManager != null) inventoryInputManager.enabled = true;
        
        // ========== MONSTER SCRIPTS ==========
        if (monsterController != null)
        {
            monsterController.enabled = false;
            if (showDebugInfo) Debug.Log("❌ Monster Controller → DISABLED");
        }
        
        // Restore stamina
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
        
        // ========== MODELS ==========
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
        
        // ========== CAMERAS ==========
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
        
        // ========== UI ==========
        if (humanUI != null) humanUI.SetActive(false);
        if (monsterUI != null) monsterUI.SetActive(true);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        
        // ========== PLAYER SCRIPTS - DISABLE ALL ==========
        if (playerMovement != null) 
        {
            playerMovement.enabled = false;
            Debug.Log("❌ Player Movement OFF");
        }
        if (inventoryToggle != null) inventoryToggle.enabled = false;
        if (inventoryInputManager != null) inventoryInputManager.enabled = false;
        
        // ========== MONSTER SCRIPTS ==========
        if (monsterController != null)
        {
            monsterController.enabled = true;
            Debug.Log("✅ Monster Controller ON");
            
            // PERBAIKAN: Set ke SLEEP MODE dulu, bukan combat!
            if (monsterAnimator != null)
            {
                monsterAnimator.SetInteger("battle", 3); // 3 = SLEEP MODE
                monsterAnimator.SetInteger("moving", 0); // Idle
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
    
    // ==========================================
    // TPP CAMERA FOLLOW SYSTEM
    // ==========================================
    
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
    
    // ==========================================
    // MONSTER MODE UPDATES
    // ==========================================
    
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
    
    // ==========================================
    // AUDIO SYSTEM
    // ==========================================
    
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
    
    // ==========================================
    // COMBAT SYSTEM
    // ==========================================
    
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
    
    // ==========================================
    // PUBLIC ACCESSORS
    // ==========================================
    
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

