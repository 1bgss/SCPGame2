using UnityEngine;

public class AutoSlidingDoor : MonoBehaviour
{
    [Header("Referensi Pintu Kiri & Kanan")]
    public Transform pintuKiri;
    public Transform pintuKanan;

    [Header("Setelan Gerakan")]
    public float slideDistance = 2f;    // Jarak pintu bergeser ke samping
    public float openSpeed = 3f;        // Kecepatan buka/tutup
    public float detectionRange = 5f;   // Jarak player agar pintu terbuka
    public Transform player;            // Referensi ke Player

    private bool isOpen = false;
    private Vector3 kiriClosedPos;
    private Vector3 kananClosedPos;
    private Vector3 kiriOpenPos;
    private Vector3 kananOpenPos;

    void Start()
    {
        // Simpan posisi awal (tertutup)
        kiriClosedPos = pintuKiri.localPosition;
        kananClosedPos = pintuKanan.localPosition;

        // Tentukan posisi terbuka (kiri ke kiri, kanan ke kanan)
        kiriOpenPos = kiriClosedPos + Vector3.left * slideDistance;
        kananOpenPos = kananClosedPos + Vector3.right * slideDistance;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance < detectionRange)
        {
            isOpen = true;  // Player dekat → buka
        }
        else
        {
            isOpen = false; // Player jauh → tutup
        }

        // Gerakan pintu halus
        if (isOpen)
        {
            pintuKiri.localPosition = Vector3.Lerp(pintuKiri.localPosition, kiriOpenPos, Time.deltaTime * openSpeed);
            pintuKanan.localPosition = Vector3.Lerp(pintuKanan.localPosition, kananOpenPos, Time.deltaTime * openSpeed);
        }
        else
        {
            pintuKiri.localPosition = Vector3.Lerp(pintuKiri.localPosition, kiriClosedPos, Time.deltaTime * openSpeed);
            pintuKanan.localPosition = Vector3.Lerp(pintuKanan.localPosition, kananClosedPos, Time.deltaTime * openSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
