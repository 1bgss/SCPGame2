using UnityEngine;

public class WakeUpEffect : MonoBehaviour
{
    public float wakeUpDuration = 3f; // durasi bangun (detik)
    public Vector3 startRotation = new Vector3(90, 0, 0); // posisi rebahan
    public Vector3 endRotation = new Vector3(0, 0, 0);   // posisi berdiri
    private float elapsedTime = 0f;
    private bool isWakingUp = true;

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
                // Di sini kamu bisa aktifkan kontrol player setelah bangun
                // GetComponent<PlayerMovement>().enabled = true;
            }
        }
    }
}
