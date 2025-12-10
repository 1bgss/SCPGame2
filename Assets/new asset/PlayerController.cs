using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float runSpeed = 14f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 300f;

    [Header("Footstep Settings")]
    public AudioSource footstepSource;
    public AudioClip walkClip;
    public AudioClip runClip;
    public float walkStepRate = 0.5f;
    public float runStepRate = 0.3f;

    [Header("Stamina Settings")]
    public PlayerStamina stamina;

    [Header("Weapon ADS Settings")]
    public Transform weaponHolder;
    public Transform hipPos;
    public Transform adsPos;
    public float adsFOV = 25f;
    public float normalFOV = 60f;
    public float adsSmooth = 10f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private bool isRunning;
    private float stepTimer = 0f;
    private bool isADS;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        if (stamina == null)
            stamina = GetComponent<PlayerStamina>();

        if (footstepSource == null)
            Debug.LogWarning("⚠️ Footstep AudioSource belum diassign!");
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleFootsteps();
        HandleWeaponADS();
    }

    // ============================
    // Movement & Jump
    // ============================
    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Movement relatif kamera
        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = right * x + forward * z;

        // Lari
        isRunning = Input.GetKey(KeyCode.LeftShift) && z > 0f && stamina.HasStamina();
        float speed = isRunning ? runSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        // Stamina
        if (isRunning)
            stamina.UseStamina(stamina.staminaDrain * Time.deltaTime);
        else
            stamina.RegenStamina(stamina.staminaRegen * Time.deltaTime);

        // Lompat
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // ============================
    // Mouse Look
    // ============================
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    // ============================
    // Footsteps
    // ============================
    private void HandleFootsteps()
    {
        if (controller == null || footstepSource == null) return;

        Vector3 horizontalVel = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        float moveAmount = horizontalVel.magnitude;

        if (!isGrounded || moveAmount == 0f) return;

        stepTimer += Time.deltaTime;
        float stepRate = isRunning ? runStepRate : walkStepRate;
        AudioClip clip = isRunning ? runClip : walkClip;

        if (stepTimer >= stepRate && !footstepSource.isPlaying)
        {
            footstepSource.clip = clip;
            footstepSource.time = Random.Range(0f, Mathf.Max(0f, clip.length - 0.5f));
            footstepSource.Play();
            stepTimer = 0f;
        }
    }

    // ============================
    // Weapon ADS
    // ============================
    private void HandleWeaponADS()
    {
        if (weaponHolder == null || playerCamera == null) return;

        isADS = Input.GetMouseButton(1);

        // Posisi lerp
        weaponHolder.localPosition = Vector3.Lerp(
            weaponHolder.localPosition,
            isADS ? adsPos.localPosition : hipPos.localPosition,
            Time.deltaTime * adsSmooth
        );

        // Rotasi senjata: hanya pitch kamera, tetap upright
        Quaternion targetRot = Quaternion.Euler(
            playerCamera.localEulerAngles.x,
            0f,
            0f
        );
        weaponHolder.localRotation = Quaternion.Slerp(
            weaponHolder.localRotation,
            targetRot,
            Time.deltaTime * adsSmooth
        );

        // Zoom FOV
        Camera.main.fieldOfView = Mathf.Lerp(
            Camera.main.fieldOfView,
            isADS ? adsFOV : normalFOV,
            Time.deltaTime * adsSmooth
        );
    }
}
