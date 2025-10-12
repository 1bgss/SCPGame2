using UnityEngine;
using UnityEngine.UI;  // untuk teks UI

public class DoorButton_Left : MonoBehaviour
{
    public DoorController_Left doorLeft;
    public GameObject player;
    public float interactDistance = 3f;

    [Header("UI Prompt")]
    public Text promptText; // assign Text UI di Canvas
    private bool isPlayerNearby;

    void Start()
    {
        if (promptText != null)
            promptText.enabled = false; // sembunyikan di awal
    }

    void Update()
    {
        if (player == null || doorLeft == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        isPlayerNearby = distance <= interactDistance;

        if (promptText != null)
            promptText.enabled = isPlayerNearby;

        if (isPlayerNearby)
        {
            if (promptText != null)
                promptText.text = "Press [E] to open/close the door";

            if (Input.GetKeyDown(KeyCode.E))
                doorLeft.ToggleDoor();
        }
    }
}
