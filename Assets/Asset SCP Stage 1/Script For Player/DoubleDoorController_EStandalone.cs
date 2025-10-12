using UnityEngine;

public class DoubleDoorController_EStandalone : MonoBehaviour
{
    [Header("Engsel Kiri & Kanan")]
    public Transform engselKiri;
    public Transform engselKanan;

    [Header("Setelan Rotasi")]
    public float openAngle = 90f;
    public float openSpeed = 3f;
    public float autoCloseDelay = 3f;
    public float interactDistance = 3f;
    public string playerTag = "Player";

    private bool isOpen = false;
    private bool isMoving = false;
    private float closeTimer;
    private Transform player;

    private Quaternion leftClosedRot, rightClosedRot;
    private Quaternion leftOpenRot, rightOpenRot;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            player = playerObj.transform;

        leftClosedRot = engselKiri.localRotation;
        rightClosedRot = engselKanan.localRotation;

        leftOpenRot = leftClosedRot * Quaternion.Euler(0, -openAngle, 0);
        rightOpenRot = rightClosedRot * Quaternion.Euler(0, openAngle, 0);
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E) && !isMoving)
            {
                ToggleDoor();
            }
        }

        if (isOpen && Time.time >= closeTimer)
            CloseDoor();

        if (isMoving)
            MoveDoors();
    }

    public void ToggleDoor()
    {
        if (isMoving) return;

        isOpen = !isOpen;
        isMoving = true;

        if (isOpen)
            closeTimer = Time.time + autoCloseDelay;
    }

    public void OpenDoor()
    {
        if (isOpen || isMoving) return;
        isOpen = true;
        isMoving = true;
        closeTimer = Time.time + autoCloseDelay;
    }

    public void CloseDoor()
    {
        if (!isOpen || isMoving) return;
        isOpen = false;
        isMoving = true;
    }

    private void MoveDoors()
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
