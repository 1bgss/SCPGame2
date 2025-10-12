using UnityEngine;

public class DoubleDoorController : MonoBehaviour
{
    [Header("Engsel Kiri & Kanan")]
    public Transform engselKiri;
    public Transform engselKanan;

    [Header("Setelan Rotasi")]
    public float openAngle = 90f;
    public float openSpeed = 3f;
    public float autoCloseDelay = 3f;

    private bool isOpen = false;
    private bool isMoving = false;
    private float closeTimer;

    private Quaternion leftClosedRot, rightClosedRot;
    private Quaternion leftOpenRot, rightOpenRot;

    void Start()
    {
        leftClosedRot = engselKiri.localRotation;
        rightClosedRot = engselKanan.localRotation;

        // Kiri buka keluar (-Y), kanan buka keluar (+Y)
        leftOpenRot = Quaternion.Euler(0, -openAngle, 0);
        rightOpenRot = Quaternion.Euler(0, openAngle, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isMoving)
        {
            isOpen = !isOpen;
            isMoving = true;
            if (isOpen)
                closeTimer = Time.time + autoCloseDelay;
        }

        if (isMoving)
            MoveDoors();

        if (isOpen && Time.time >= closeTimer)
        {
            isOpen = false;
            isMoving = true;
        }
    }

    void MoveDoors()
    {
        Quaternion targetLeft = isOpen ? leftOpenRot : leftClosedRot;
        Quaternion targetRight = isOpen ? rightOpenRot : rightClosedRot;

        engselKiri.localRotation = Quaternion.Lerp(engselKiri.localRotation, targetLeft, Time.deltaTime * openSpeed);
        engselKanan.localRotation = Quaternion.Lerp(engselKanan.localRotation, targetRight, Time.deltaTime * openSpeed);

        if (Quaternion.Angle(engselKiri.localRotation, targetLeft) < 0.5f &&
            Quaternion.Angle(engselKanan.localRotation, targetRight) < 0.5f)
        {
            isMoving = false;
        }
    }
}
