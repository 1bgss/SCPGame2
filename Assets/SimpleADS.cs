using UnityEngine;

public class SimpleADS : MonoBehaviour
{
    [Header("=== Senjata Posisi ===")]
    public Transform hipPosition;       // posisi idle
    public Transform adsPosition;       // posisi ADS (di tengah crosshair)
    public float adsSpeed = 10f;        // kecepatan transisi

    [Header("=== Kamera FOV ===")]
    public Camera mainCam;              // kamera FPP
    public float normalFOV = 60f;
    public float adsFOV = 45f;

    [Header("=== Sensitivity ===")]
    public PlayerMovement playerMovement;
    public float adsSensitivity = 200f;
    private float defaultSensitivity;

    private bool isAiming = false;

    void Start()
    {
        if (playerMovement != null)
            defaultSensitivity = playerMovement.mouseSensitivity;
    }

    void Update()
    {
        HandleADS();
    }

    void HandleADS()
    {
        // Tahan klik kanan (ADS)
        isAiming = Input.GetMouseButton(1);

        // Tentukan target posisi local
        Transform target = isAiming ? adsPosition : hipPosition;

        // Smooth local position & rotation
        transform.localPosition = Vector3.Lerp(transform.localPosition, target.localPosition, adsSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, target.localRotation, adsSpeed * Time.deltaTime);

        // Smooth Kamera FOV (zoom)
        mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, isAiming ? adsFOV : normalFOV, adsSpeed * Time.deltaTime);

        // Sensitivity menurun saat ADS
        if (playerMovement != null)
            playerMovement.mouseSensitivity = isAiming ? adsSensitivity : defaultSensitivity;
    }
}
