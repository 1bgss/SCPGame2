using UnityEngine;

public class WakeUpEffect : MonoBehaviour
{
    public float wakeUpDuration = 3f; // durasi bangun (detik)
    public Vector3 startRotation = new Vector3(90, 0, 0); // posisi rebahan
    public Vector3 endRotation = new Vector3(0, 0, 0);   // posisi berdiri
    private float elapsedTime = 0f;
    private bool isWakingUp = false; // awalnya false (belum bangun)

    void Start()
    {
        transform.localEulerAngles = startRotation;
    }

    void Update()
    {
        if (isWakingUp)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / wakeUpDuration);
            transform.localEulerAngles = Vector3.Lerp(startRotation, endRotation, t);

            if (t >= 1f)
            {
                isWakingUp = false;
                Debug.Log("🧍 Pemain sudah bangun!");
                // GetComponent<PlayerMovement>().enabled = true;
            }
        }
    }

    // 🔹 Dipanggil setelah tutorial selesai
    public void StartWakeUp()
    {
        isWakingUp = true;
        elapsedTime = 0f;
        Debug.Log("💤 Mulai bangun setelah tutorial!");
    }
}
