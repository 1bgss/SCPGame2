using UnityEngine;
using TMPro;  // pakai TextMeshPro

public class DoorButton_Left : MonoBehaviour
{
    public DoorController_Left doorLeft;
    public GameObject player;
    public float interactDistance = 3f;

    [Header("UI Prompt")]
    public TMP_Text promptText; // assign TMP_Text di Canvas
    private bool isPlayerNearby;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip openClip; // suara pintu terbuka

    void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false); // sembunyikan di awal
    }

    void Update()
    {
        if (player == null || doorLeft == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        isPlayerNearby = distance <= interactDistance;

        if (promptText != null)
            promptText.gameObject.SetActive(isPlayerNearby);

        if (isPlayerNearby)
        {
            if (promptText != null)
                promptText.text = "Press [E] to open/close the door";

            if (Input.GetKeyDown(KeyCode.E))
            {
                doorLeft.ToggleDoor();
                PlayDoorSound();
            }
        }
    }

    private void PlayDoorSound()
    {
        if (audioSource != null && openClip != null)
            audioSource.PlayOneShot(openClip);
    }
}
