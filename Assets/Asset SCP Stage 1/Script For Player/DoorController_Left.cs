using UnityEngine;

public class DoorController_Left : MonoBehaviour
{
    [Header("Door settings")]
    public float openAngle = 90f;
    public float openSpeed = 3f;
    public bool startsOpen = false;

    [Header("Audio (optional)")]
    public AudioClip openClip;
    public AudioClip closeClip;
    private AudioSource audioSource;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0f, -openAngle, 0f);
        isOpen = startsOpen;

        transform.localRotation = isOpen ? openRotation : closedRotation;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (openClip != null || closeClip != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        Quaternion target = isOpen ? openRotation : closedRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * openSpeed);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        PlaySound(isOpen ? openClip : closeClip);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.clip = clip;
        audioSource.Play();
    }
}
