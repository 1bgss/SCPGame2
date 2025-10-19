using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float runSpeed = 14f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 300f;

    [Header("Audio Settings")]
    public AudioSource footstepSource;  
    public AudioClip walkClip;          
    public AudioClip runClip;           
    public float walkStepRate = 0.5f;   
    public float runStepRate = 0.3f;    

    [Header("Stamina Settings")]
    public PlayerStamina stamina;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private bool isRunning;
    private float stepTimer = 0f;

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
        if (controller == null || stamina == null) return;

        // --- Ground Check ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // --- Input Movement ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        // --- Lari aktif jika W + Shift + stamina > 0 ---
        isRunning = Input.GetKey(KeyCode.LeftShift) && z > 0f && stamina.HasStamina();
        float speed = isRunning ? runSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        // --- Kelola stamina ---
        if (isRunning)
            stamina.UseStamina(stamina.staminaDrain * Time.deltaTime);
        else
            stamina.RegenStamina(stamina.staminaRegen * Time.deltaTime);

        // --- Footsteps ---
        HandleFootsteps(move.magnitude);

        // --- Lompat ---
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // --- Gravity ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Mouse Look ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleFootsteps(float moveAmount)
    {
        if (!isGrounded || moveAmount == 0f || footstepSource == null) return;

        stepTimer += Time.deltaTime;
        float stepRate = isRunning ? runStepRate : walkStepRate;
        AudioClip clip = isRunning ? runClip : walkClip;

        // pastikan suara tidak bertumpuk
        if (stepTimer >= stepRate && !footstepSource.isPlaying)
        {
            footstepSource.clip = clip;
            footstepSource.time = Random.Range(0f, Mathf.Max(0f, clip.length - 0.5f));
            footstepSource.Play();
            stepTimer = 0f;
        }
    }
}
