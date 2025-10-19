using UnityEngine;

public class DoubleDoorController_EAdaptive : MonoBehaviour
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

    [Header("Audio")]
    public AudioSource audioSource; // Satu-satunya audio source
    public AudioClip doorSoundClip; // Bunyi buka/tutup pintu

    private bool isOpen = false;
    private bool isMoving = false;
    private float closeTimer;
    private Transform player;

    private Quaternion leftClosedRot, rightClosedRot;
    private Quaternion leftOpenForwardRot, rightOpenForwardRot;   // buka ke depan
    private Quaternion leftOpenBackwardRot, rightOpenBackwardRot; // buka ke belakang
    private bool openDirectionForward = true;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            player = playerObj.transform;

        leftClosedRot = engselKiri.localRotation;
        rightClosedRot = engselKanan.localRotation;

        // Rotasi buka ke arah depan (default)
        leftOpenForwardRot = leftClosedRot * Quaternion.Euler(0, -openAngle, 0);
        rightOpenForwardRot = rightClosedRot * Quaternion.Euler(0, openAngle, 0);

        // Rotasi buka ke arah belakang (kebalikan)
        leftOpenBackwardRot = leftClosedRot * Quaternion.Euler(0, openAngle, 0);
        rightOpenBackwardRot = rightClosedRot * Quaternion.Euler(0, -openAngle, 0);

        // AudioSource otomatis dibuat jika belum ada
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E) && !isMoving)
            {
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                float dot = Vector3.Dot(transform.forward, dirToPlayer);
                openDirectionForward = dot > 0;
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

        // Mainkan suara pintu
        PlayDoorSound();

        if (isOpen)
            closeTimer = Time.time + autoCloseDelay;
    }

    public void OpenDoor()
    {
        if (isOpen || isMoving) return;
        isOpen = true;
        isMoving = true;
        closeTimer = Time.time + autoCloseDelay;

        PlayDoorSound();
    }

    public void CloseDoor()
    {
        if (!isOpen || isMoving) return;
        isOpen = false;
        isMoving = true;

        PlayDoorSound();
    }

    private void MoveDoors()
    {
        Quaternion targetLeft, targetRight;

        if (isOpen)
        {
            if (openDirectionForward)
            {
                targetLeft = leftOpenForwardRot;
                targetRight = rightOpenForwardRot;
            }
            else
            {
                targetLeft = leftOpenBackwardRot;
                targetRight = rightOpenBackwardRot;
            }
        }
        else
        {
            targetLeft = leftClosedRot;
            targetRight = rightClosedRot;
        }

        engselKiri.localRotation = Quaternion.Lerp(engselKiri.localRotation, targetLeft, Time.deltaTime * openSpeed);
        engselKanan.localRotation = Quaternion.Lerp(engselKanan.localRotation, targetRight, Time.deltaTime * openSpeed);

        if (Quaternion.Angle(engselKiri.localRotation, targetLeft) < 0.5f &&
            Quaternion.Angle(engselKanan.localRotation, targetRight) < 0.5f)
        {
            isMoving = false;
        }
    }

    void PlayDoorSound()
    {
        if (doorSoundClip != null && audioSource != null)
            audioSource.PlayOneShot(doorSoundClip, 1f); // mainkan di atas clip lain jika ada
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
