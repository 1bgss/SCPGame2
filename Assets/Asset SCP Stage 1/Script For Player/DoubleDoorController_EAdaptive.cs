using UnityEngine;
using TMPro;

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
    public AudioSource audioSource;
    public AudioClip doorOpenClip;
    public AudioClip doorCloseClip;

    [Header("UI Interaksi")]
    public TextMeshProUGUI interactTextFront;
    public TextMeshProUGUI interactTextBack;

    private bool isOpen = false;
    private bool isMoving = false;
    private float closeTimer;
    private Transform player;

    private Quaternion leftClosedRot, rightClosedRot;
    private Quaternion leftOpenForwardRot, rightOpenForwardRot;
    private Quaternion leftOpenBackwardRot, rightOpenBackwardRot;
    private bool openDirectionForward = true;

    private bool frontVisible = false;
    private bool backVisible = false;
    private float textHideThreshold;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            player = playerObj.transform;

        leftClosedRot = engselKiri.localRotation;
        rightClosedRot = engselKanan.localRotation;

        leftOpenForwardRot = leftClosedRot * Quaternion.Euler(0, -openAngle, 0);
        rightOpenForwardRot = rightClosedRot * Quaternion.Euler(0, openAngle, 0);

        leftOpenBackwardRot = leftClosedRot * Quaternion.Euler(0, openAngle, 0);
        rightOpenBackwardRot = rightClosedRot * Quaternion.Euler(0, -openAngle, 0);

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        textHideThreshold = interactDistance + 0.02f;

        if (interactTextFront != null) interactTextFront.gameObject.SetActive(false);
        if (interactTextBack != null) interactTextBack.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dirToPlayer);

        bool isFront = dot > 0; // true kalau player di depan pintu

        // 🔹 Tampilkan text depan/belakang sesuai posisi
        HandleTextVisibility(distance, isFront);

        // 🔹 Input interaksi
        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E) && !isMoving)
        {
            openDirectionForward = isFront;
            ToggleDoor();
        }

        if (isOpen && Time.time >= closeTimer)
            CloseDoor();

        if (isMoving)
            MoveDoors();
    }

    private void HandleTextVisibility(float distance, bool isFront)
    {
        // kalau pintu terbuka, semua teks disembunyikan
        if (isOpen)
        {
            HideAllTexts();
            return;
        }

        // tampilkan teks depan/belakang sesuai posisi dan jarak
        if (distance <= interactDistance)
        {
            if (isFront)
            {
                SetTextVisibility(interactTextFront, ref frontVisible, true);
                SetTextVisibility(interactTextBack, ref backVisible, false);
            }
            else
            {
                SetTextVisibility(interactTextFront, ref frontVisible, false);
                SetTextVisibility(interactTextBack, ref backVisible, true);
            }
        }
        else if (distance > textHideThreshold)
        {
            HideAllTexts();
        }
    }

    private void SetTextVisibility(TextMeshProUGUI text, ref bool state, bool visible)
    {
        if (text == null || state == visible) return;
        text.gameObject.SetActive(visible);
        state = visible;
    }

    private void HideAllTexts()
    {
        if (interactTextFront != null && frontVisible)
        {
            interactTextFront.gameObject.SetActive(false);
            frontVisible = false;
        }
        if (interactTextBack != null && backVisible)
        {
            interactTextBack.gameObject.SetActive(false);
            backVisible = false;
        }
    }

    public void ToggleDoor()
    {
        if (isMoving) return;

        HideAllTexts(); // sembunyikan teks begitu pintu dibuka

        isOpen = !isOpen;
        isMoving = true;

        if (isOpen)
        {
            PlayDoorOpenSound();
            closeTimer = Time.time + autoCloseDelay;
        }
        else
        {
            PlayDoorCloseSound();
        }
    }

    public void OpenDoor()
    {
        if (isOpen || isMoving) return;
        isOpen = true;
        isMoving = true;
        closeTimer = Time.time + autoCloseDelay;
        PlayDoorOpenSound();
    }

    public void CloseDoor()
    {
        if (!isOpen || isMoving) return;
        isOpen = false;
        isMoving = true;
        PlayDoorCloseSound();
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

    void PlayDoorOpenSound()
    {
        if (doorOpenClip != null && audioSource != null)
            audioSource.PlayOneShot(doorOpenClip, 1f);
    }

    void PlayDoorCloseSound()
    {
        if (doorCloseClip != null && audioSource != null)
            audioSource.PlayOneShot(doorCloseClip, 1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
