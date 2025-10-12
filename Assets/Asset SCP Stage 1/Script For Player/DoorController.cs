// DoorController.cs
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door settings")]
    public float openAngle = 90f;        // derajat saat terbuka
    public float openSpeed = 3f;         // kecepatan buka/tutup
    public bool startsOpen = false;

    [Header("Audio (optional)")]
    public AudioClip openClip;
    public AudioClip closeClip;
    private AudioSource audioSource;

    // internal
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen;

    void Start()
    {
        // simpan rotasi awal sebagai "closed"
        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
        isOpen = startsOpen;

        // jika mulai buka, langsung set ke openRotation
        transform.localRotation = isOpen ? openRotation : closedRotation;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (openClip != null || closeClip != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // target rotasi bergantung isOpen
        Quaternion target = isOpen ? openRotation : closedRotation;
        // rotasi mulus
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * openSpeed);
    }

    // panggil untuk toggle buka/tutup
    public void ToggleDoor()
    {
        isOpen = !isOpen;
        PlaySound(isOpen ? openClip : closeClip);
    }

    // panggil untuk set open/close langsung
    public void SetOpen(bool open)
    {
        if (isOpen == open) return;
        isOpen = open;
        PlaySound(isOpen ? openClip : closeClip);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.clip = clip;
        audioSource.Play();
    }
}
