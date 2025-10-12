using UnityEngine;

public class DoorButton_Interact : MonoBehaviour
{
    [Header("Door Reference")]
    public DoorController door;       // Pintu yang mau dibuka

    [Header("Interaction Settings")]
    public float interactDistance = 3f;   // Jarak maksimum untuk bisa membuka pintu
    public KeyCode interactKey = KeyCode.E;  // Tombol yang digunakan

    private Transform player;

    void Start()
    {
        // Cari player otomatis (pastikan Player punya tag "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("Player not found! Please tag your player as 'Player'.");
    }

    void Update()
    {
        if (player == null || door == null) return;

        // Hitung jarak antara player dan tombol
        float distance = Vector3.Distance(player.position, transform.position);

        // Jika dalam jarak dan tekan tombol E
        if (distance <= interactDistance && Input.GetKeyDown(interactKey))
        {
            door.ToggleDoor();
            Debug.Log("Door toggled via E key");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Gambar lingkaran jarak interaksi di editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
