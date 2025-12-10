using UnityEngine;
using System.Collections;

public class THC1_ctrl : MonoBehaviour {
    
    // Components
    private Animator anim;
    private CharacterController controller;
    
    // Mode States
    public enum MoveMode { Combat, Crawl, Sleep }
    private MoveMode currentMode = MoveMode.Sleep;
    
    // Speed Settings
    [Header("Movement Speed Settings")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 6.0f;
    public float crawlSpeed = 1.5f;
    public float crawlRunSpeed = 4.0f;
    public float turnSpeed = 60.0f;
    public float gravity = 20.0f;
    
    // Mouse Camera Settings
    [Header("Mouse Camera Settings")]
    public bool useMouseCamera = true;
    public float mouseSensitivity = 2.0f;
    public float verticalLookLimit = 80f;
    public Transform cameraTransform;
    private float rotationY = 0f;
    private float cameraRotationX = 0f;
    
    // Ground Check Settings
    [Header("Ground Check Settings")]
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer = -1;
    private bool isGrounded = false;
    
    // Movement Variables
    private Vector3 moveDirection = Vector3.zero;
    private float verticalVelocity = 0f;
    private bool isRunning = false;
    private bool isWalking = false;
    private int currentMovementAnim = 0;
    
    // Action System
    private bool isInHeavyAction = false;
    private float heavyActionTimer = 0f;
    
    // Combo System
    [Header("Combo Settings")]
    public float comboResetTime = 2.0f;
    private float lastAttackTime = 0f;
    private int comboCounter = 0;
    
    // TRANSFORMATION COMPATIBILITY (BARU!)
    [Header("Transformation System")]
    public MonsterTransformationManager transformManager;
    private bool isEnabled = false;
    
    void Start() {
        // Get Components
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        
        // Auto-find transformation manager
        if (transformManager == null)
        {
            transformManager = FindObjectOfType<MonsterTransformationManager>();
        }
        
        // Auto-find camera jika belum di-set
        if (cameraTransform == null) {
            cameraTransform = Camera.main?.transform;
            if (cameraTransform != null) {
                Debug.Log("Camera auto-found: " + cameraTransform.name);
            }
        }
        
        // Check if components exist
        if (anim == null) {
            Debug.LogError("Animator not found!");
        } else {
            Debug.Log("Animator found: OK");
        }
        
        if (controller == null) {
            Debug.LogError("CharacterController not found!");
        } else {
            Debug.Log("CharacterController found: OK");
        }
        
        // Set default ke sleep mode
        if (anim != null)
        {
            anim.SetInteger("battle", 3);
            anim.SetInteger("moving", 0);
        }
        
        Debug.Log("=== ZOMBIE CONTROLLER INITIALIZED ===");
        Debug.Log("Starting in SLEEP MODE - Press 1 or 2 to wake up!");
        Debug.Log("Mouse Camera: " + (useMouseCamera ? "ENABLED" : "DISABLED"));
    }
    
    void OnEnable()
    {
        isEnabled = true;
        
        // PENTING: Jangan lock cursor di sini!
        // Biar TransformationManager yang handle
        
        Debug.Log("╔════════════════════════════════════════╗");
        Debug.Log("║   MONSTER CONTROLLER ENABLED           ║");
        Debug.Log("╚════════════════════════════════════════╝");
        
        // Set ke Combat mode saat monster aktif
        if (anim != null)
        {
            StartCoroutine(SetCombatModeDelayed());
        }
    }
    
    IEnumerator SetCombatModeDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        SwitchToMode(MoveMode.Combat, 1);
        Debug.Log("→ Auto-switched to COMBAT MODE");
    }
    
    void OnDisable()
    {
        isEnabled = false;
        Debug.Log("╔════════════════════════════════════════╗");
        Debug.Log("║   MONSTER CONTROLLER DISABLED          ║");
        Debug.Log("╚════════════════════════════════════════╝");
    }
    
    void Update() {
        // CRITICAL: Jangan proses input kalau script disabled atau manager mode human
        if (!isEnabled) return;
        
        if (transformManager != null && !transformManager.IsMonsterMode())
        {
            return; // Jangan proses input sama sekali
        }
        
        // Check ground PERTAMA sebelum yang lain
        CheckGroundStatus();
        
        // Handle mouse camera rotation
        if (useMouseCamera) {
            HandleMouseCamera();
        }
        
        // Update all systems
        HandleModeSwitch();
        
        if (!isInHeavyAction) {
            HandleMovement();
            HandleActions();
        } else {
            UpdateHeavyAction();
        }
        
        ApplyMovement();
        UpdateComboSystem();
    }
    
    // ==========================================
    // MOUSE CAMERA SYSTEM
    // ==========================================
    void HandleMouseCamera() {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Rotate character (Y-axis)
        rotationY += mouseX;
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
        
        // Rotate camera (X-axis) jika ada camera reference
        if (cameraTransform != null) {
            cameraRotationX -= mouseY;
            cameraRotationX = Mathf.Clamp(cameraRotationX, -verticalLookLimit, verticalLookLimit);
            cameraTransform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
        }
    }
    
    // ==========================================
    // GROUND CHECK SYSTEM
    // ==========================================
    void CheckGroundStatus() {
        if (controller == null) return;
        
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        Ray groundRay = new Ray(rayStart, Vector3.down);
        
        isGrounded = Physics.Raycast(groundRay, groundCheckDistance, groundLayer);
        
        if (!isGrounded) {
            isGrounded = Physics.SphereCast(
                rayStart, 
                controller.radius * 0.9f, 
                Vector3.down, 
                out RaycastHit hit, 
                groundCheckDistance, 
                groundLayer
            );
        }
        
        if (!isGrounded) {
            isGrounded = controller.isGrounded;
        }
        
        if (Debug.isDebugBuild) {
            Debug.DrawRay(rayStart, Vector3.down * groundCheckDistance, 
                isGrounded ? Color.green : Color.red);
        }
    }
    
    void HandleModeSwitch() {
        // PERBAIKAN: Tombol mode switch sekarang aman
        // Tidak akan conflict dengan transform key
        
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2)) {
            SwitchToMode(MoveMode.Combat, 1);
            Debug.Log("→ Switched to Combat Mode");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SwitchToMode(MoveMode.Crawl, 2);
            Debug.Log("→ Switched to Crawl Mode");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            SwitchToMode(MoveMode.Sleep, 3);
            Debug.Log("→ Switched to Sleep Mode");
        }
    }
    
    void SwitchToMode(MoveMode newMode, int battleState) {
        currentMode = newMode;
        if (anim != null)
        {
            anim.SetInteger("battle", battleState);
            anim.SetInteger("moving", 0);
        }
        isInHeavyAction = false;
        heavyActionTimer = 0f;
        comboCounter = 0;
        currentMovementAnim = 0;
    }
    
    void HandleMovement() {
        if (currentMode == MoveMode.Sleep) {
            if (anim != null) anim.SetInteger("moving", 0);
            currentMovementAnim = 0;
            return;
        }
        
        bool isMovingForward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool isMovingBackward = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        isWalking = isMovingForward || isMovingBackward;
        
        // Combat Mode Movement
        if (currentMode == MoveMode.Combat) {
            int targetMovementAnim = 0;
            
            if (isWalking) {
                if (isRunning) {
                    targetMovementAnim = 2; // Run
                } else {
                    targetMovementAnim = 1; // Walk
                }
            } else {
                targetMovementAnim = 0; // Idle
            }
            
            if (currentMovementAnim != targetMovementAnim) {
                currentMovementAnim = targetMovementAnim;
                if (anim != null) anim.SetInteger("moving", currentMovementAnim);
            }
        }
        
        // Crawl Mode Movement
        else if (currentMode == MoveMode.Crawl) {
            int targetMovementAnim = 0;
            
            if (isWalking) {
                if (isRunning) {
                    targetMovementAnim = 9; // Crawl Fast
                } else {
                    targetMovementAnim = 3; // Crawl Normal
                }
            } else {
                targetMovementAnim = 0; // Crawl Idle
            }
            
            if (currentMovementAnim != targetMovementAnim) {
                currentMovementAnim = targetMovementAnim;
                if (anim != null) anim.SetInteger("moving", currentMovementAnim);
            }
        }
    }
    
    void HandleActions() {
        // COMBAT MODE ACTIONS
        if (currentMode == MoveMode.Combat) {
            
            if (Input.GetKeyDown(KeyCode.E)) {
                TriggerAction(8);
                Debug.Log("Roar!");
            }
            
            if (Input.GetKeyDown(KeyCode.X)) {
                TriggerAction(7);
                UpdateCombo();
                Debug.Log("Bite! Combo: " + comboCounter);
            }
            
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Q)) {
                TriggerAction(4);
                UpdateCombo();
                Debug.Log("Attack 1! Combo: " + comboCounter);
            }
            
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.R)) {
                TriggerAction(5);
                UpdateCombo();
                Debug.Log("Attack 2! Combo: " + comboCounter);
            }
            
            if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.F)) {
                TriggerAction(6);
                UpdateCombo();
                Debug.Log("Attack 3! Combo: " + comboCounter);
            }
            
            if (Input.GetKeyDown(KeyCode.U)) {
                int hitType = Random.Range(0, 2);
                int hitAnimation = (hitType == 0) ? 10 : 11;
                TriggerAction(hitAnimation);
                Debug.Log("Hit " + (hitType + 1) + "!");
            }
            
            if (Input.GetKeyDown(KeyCode.Y)) {
                TriggerAction(11);
                UpdateCombo();
                Debug.Log("Power Hit! Combo: " + comboCounter);
            }
            
            if (Input.GetKey(KeyCode.P)) {
                if (anim != null) anim.SetInteger("moving", 14);
            } else if (Input.GetKeyUp(KeyCode.P)) {
                TriggerAction(15);
            }
            
            // JUMP
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
                TriggerAction(16);
                verticalVelocity = 8f;
                Debug.Log("Jump!");
            }
            
            if (Input.GetKeyDown(KeyCode.O)) {
                StartHeavyAction(12, 3.5f);
                Debug.Log("Death 1");
            }
            
            if (Input.GetKeyDown(KeyCode.I)) {
                StartHeavyAction(13, 3.5f);
                Debug.Log("Death 2");
            }
        }
        
        // CRAWL MODE ACTIONS
        else if (currentMode == MoveMode.Crawl) {
            
            if (Input.GetKey(KeyCode.Z)) {
                if (anim != null) anim.SetInteger("moving", 17);
            } else if (Input.GetKeyUp(KeyCode.Z)) {
                if (anim != null) anim.SetInteger("moving", currentMovementAnim);
            }
            
            if (Input.GetKeyDown(KeyCode.V)) {
                TriggerAction(18);
                UpdateCombo();
                Debug.Log("Crawl Bite! Combo: " + comboCounter);
            }
            
            if (Input.GetKeyDown(KeyCode.X)) {
                TriggerAction(7);
                UpdateCombo();
                Debug.Log("Bite (Crawl Mode)! Combo: " + comboCounter);
            }
        }
    }
    
    void TriggerAction(int animationIndex) {
        if (anim != null) anim.SetInteger("moving", animationIndex);
        StartCoroutine(ReturnToMovementAfterDelay(0.05f));
    }
    
    IEnumerator ReturnToMovementAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        
        if (currentMode != MoveMode.Sleep && !isInHeavyAction && anim != null) {
            anim.SetInteger("moving", currentMovementAnim);
        }
    }
    
    void StartHeavyAction(int animationIndex, float duration) {
        if (anim != null) anim.SetInteger("moving", animationIndex);
        isInHeavyAction = true;
        heavyActionTimer = duration;
        currentMovementAnim = 0;
    }
    
    void UpdateHeavyAction() {
        if (heavyActionTimer > 0) {
            heavyActionTimer -= Time.deltaTime;
            if (heavyActionTimer <= 0) {
                isInHeavyAction = false;
                if (anim != null) anim.SetInteger("moving", 0);
            }
        }
    }
    
    void UpdateCombo() {
        if (Time.time - lastAttackTime < comboResetTime) {
            comboCounter++;
        } else {
            comboCounter = 1;
        }
        lastAttackTime = Time.time;
    }
    
    void UpdateComboSystem() {
        if (Time.time - lastAttackTime > comboResetTime && comboCounter > 0) {
            comboCounter = 0;
        }
    }
    
    void ApplyMovement() {
        float currentSpeed = 0f;
        
        if (isInHeavyAction || currentMode == MoveMode.Sleep) {
            currentSpeed = 0f;
        }
        else if (currentMode == MoveMode.Combat) {
            if (isRunning && isWalking) {
                currentSpeed = runSpeed;
            } else if (isWalking) {
                currentSpeed = walkSpeed;
            }
        } else if (currentMode == MoveMode.Crawl) {
            if (isRunning && isWalking) {
                currentSpeed = crawlRunSpeed;
            } else if (isWalking) {
                currentSpeed = crawlSpeed;
            }
        }
        
        // Forward/Backward movement
        float vertical = Input.GetAxis("Vertical");
        
        // STRAFING - A/D untuk gerak samping
        float horizontal = Input.GetAxis("Horizontal");
        
        if (isGrounded) {
            // Movement relatif terhadap rotasi karakter
            Vector3 forward = transform.forward * vertical;
            Vector3 right = transform.right * horizontal;
            moveDirection = (forward + right).normalized * currentSpeed;
            
            if (verticalVelocity < 0) {
                verticalVelocity = -2f;
            }
        } else {
            Vector3 forward = transform.forward * vertical;
            Vector3 right = transform.right * horizontal;
            Vector3 horizontalMove = (forward + right).normalized * currentSpeed;
            moveDirection.x = horizontalMove.x;
            moveDirection.z = horizontalMove.z;
        }
        
        // Apply gravity
        verticalVelocity -= gravity * Time.deltaTime;
        moveDirection.y = verticalVelocity;
        
        // Move character
        if (controller != null) {
            controller.Move(moveDirection * Time.deltaTime);
        }
    }
    
    // ==========================================
    // PUBLIC HELPER FUNCTIONS
    // ==========================================
    
    public bool IsInHeavyAction() {
        return isInHeavyAction;
    }
    
    public MoveMode GetCurrentMode() {
        return currentMode;
    }
    
    public int GetComboCount() {
        return comboCounter;
    }
    
    public void ForceIdle() {
        if (anim != null) anim.SetInteger("moving", 0);
        isInHeavyAction = false;
        heavyActionTimer = 0f;
    }
    
    public void SetSpeed(float walk, float run) {
        walkSpeed = walk;
        runSpeed = run;
    }
    
    public bool IsMoving() {
        return isWalking;
    }
    
    public bool IsRunning() {
        return isRunning;
    }
    
    public bool IsGroundedCustom() {
        return isGrounded;
    }
    
    public void SetMouseSensitivity(float sensitivity) {
        mouseSensitivity = sensitivity;
    }
    
    public void ToggleMouseCamera() {
        useMouseCamera = !useMouseCamera;
    }
    
    // ==========================================
    // DEBUG VISUALIZATION
    // ==========================================
    
    void OnDrawGizmos() {
        if (controller != null) {
            Vector3 center = transform.position + Vector3.up * 0.1f;
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(center + Vector3.down * groundCheckDistance, controller.radius * 0.9f);
            Gizmos.DrawLine(center, center + Vector3.down * groundCheckDistance);
        }
    }
    
    void OnGUI() {
        if (Debug.isDebugBuild && isEnabled) {
            GUI.Label(new Rect(10, 10, 300, 20), "MONSTER MODE: " + currentMode.ToString());
            GUI.Label(new Rect(10, 30, 300, 20), "Grounded: " + (isGrounded ? "YES" : "NO"));
            GUI.Label(new Rect(10, 50, 300, 20), "Vertical Velocity: " + verticalVelocity.ToString("F2"));
            GUI.Label(new Rect(10, 70, 300, 20), "Combo: " + comboCounter);
            GUI.Label(new Rect(10, 90, 300, 20), "Heavy Action: " + (isInHeavyAction ? "Yes" : "No"));
            GUI.Label(new Rect(10, 110, 300, 20), "Running: " + (isRunning ? "Yes" : "No"));
            GUI.Label(new Rect(10, 130, 300, 20), "Current Anim: " + currentMovementAnim);
        }
    }
}

/*
═══════════════════════════════════════════════════════════════════════════════
                PERBAIKAN - TRANSFORMATION COMPATIBLE
═══════════════════════════════════════════════════════════════════════════════
✓ Auto-detect TransformationManager
✓ OnEnable/OnDisable untuk proper state management
✓ Tidak conflict dengan transform key
✓ Auto set Combat mode saat monster aktif
✓ Cursor handling diserahkan ke TransformationManager
═══════════════════════════════════════════════════════════════════════════════
*/