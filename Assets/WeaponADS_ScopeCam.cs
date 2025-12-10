using UnityEngine;

public class WeaponADS_ScopeCam : MonoBehaviour
{
    [Header("=== Senjata Pos ===")]
    public Transform hipPosition;
    public Transform adsPosition;
    public float adsSpeed = 8f;

    [Header("=== Kamera ===")]
    public Camera fppCamera;      // kamera normal
    public Camera scopeCamera;    // kamera kecil di scope
    public Camera tppCamera;      // untuk monster

    [Header("=== Sensitivity ===")]
    public PlayerMovement playerMovement;
    public float adsSensitivity = 200f;
    private float defaultSensitivity;

    private bool isAiming = false;
    private bool isMonster = false;

    [Header("=== Input Key ===")]
    public KeyCode aimKey = KeyCode.Mouse1; // klik kanan
    public KeyCode monsterKey = KeyCode.T;  // toggle human/monster

    void Start()
    {
        if (playerMovement != null)
            defaultSensitivity = playerMovement.mouseSensitivity;

        // Pastikan di awal sebagai manusia
        SetHumanCamera();
    }

    void Update()
    {
        HandleMonsterSwitch();
        HandleADS();
    }

    void HandleMonsterSwitch()
    {
        if (Input.GetKeyDown(monsterKey))
        {
            isMonster = !isMonster;

            if (isMonster)
                SetMonsterCamera();
            else
                SetHumanCamera();
        }
    }

    void SetMonsterCamera()
    {
        fppCamera.gameObject.SetActive(false);
        scopeCamera.gameObject.SetActive(false);
        tppCamera.gameObject.SetActive(true);
    }

    void SetHumanCamera()
    {
        fppCamera.gameObject.SetActive(true);
        scopeCamera.gameObject.SetActive(false);
        tppCamera.gameObject.SetActive(false);
    }

    void HandleADS()
    {
        if (isMonster) return; // MONSTER GA BISA ADS

        // klik kanan tekan/lepas
        if (Input.GetKeyDown(aimKey)) isAiming = true;
        if (Input.GetKeyUp(aimKey)) isAiming = false;

        // posisi senjata lrp
        Transform target = isAiming ? adsPosition : hipPosition;
        transform.position = Vector3.Lerp(transform.position, target.position, adsSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, adsSpeed * Time.deltaTime);

        // Kamera switching
        fppCamera.gameObject.SetActive(!isAiming);
        scopeCamera.gameObject.SetActive(isAiming);

        // Sensitivity
        playerMovement.mouseSensitivity = isAiming ? adsSensitivity : defaultSensitivity;
    }
}
