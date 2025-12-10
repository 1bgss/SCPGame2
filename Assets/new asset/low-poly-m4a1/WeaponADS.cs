using UnityEngine;

public class WeaponADS : MonoBehaviour
{
    [Header("=== Position Settings ===")]
    public Transform adsPosition; // posisi senjata saat ADS (depan kamera)
    public Transform hipPosition; // posisi senjata biasa (idle)

    [Header("=== Animation Settings ===")]
    public float adsSpeed = 8f; // semakin besar semakin cepat

    [Header("=== Camera Settings ===")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float adsFOV = 45f; // zoom dikit kayak PUBG FPP

    [Header("=== Mouse Sensitivity ===")]
    public PlayerMovement playerMovement;
    public float adsSensitivity = 200f; // lebih berat saat scope
    private float defaultSensitivity;

    private bool isAiming = false;

    void Start()
    {
        if (playerMovement != null)
            defaultSensitivity = playerMovement.mouseSensitivity;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            isAiming = true;
        if (Input.GetMouseButtonUp(1))
            isAiming = false;

        HandleADS();
    }

    void HandleADS()
    {
        // === Lerp Senjata ===
        Transform target = isAiming ? adsPosition : hipPosition;
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * adsSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * adsSpeed);

        // === Ubah FOV Kamera ===
        float targetFov = isAiming ? adsFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov, Time.deltaTime * adsSpeed);

        // === Ubah Mouse Sensitivity ===
        if (playerMovement != null)
            playerMovement.mouseSensitivity = isAiming ? adsSensitivity : defaultSensitivity;
    }
}
