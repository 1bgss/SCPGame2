using UnityEngine;

public class PlayerDeathEffect : MonoBehaviour
{
    public float fallRotationDuration = 1f; // durasi jatuh
    public Vector3 fallRotation = new Vector3(90, 0, 0); // posisi rebah saat mati
    private bool isDead = false;
    private float elapsedTime = 0f;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = true; // normalnya player dikontrol manual
        startRotation = transform.rotation;
        targetRotation = Quaternion.Euler(fallRotation);
    }

    void Update()
    {
        if (isDead)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fallRotationDuration);

            // interpolasi rotasi jatuh
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            // setelah jatuh selesai, biar physics aktif
            if (t >= 1f)
            {
                rb.isKinematic = false; // player bisa jatuh di lantai
            }
        }
    }

    // panggil ini saat kena musuh
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        elapsedTime = 0f;
        startRotation = transform.rotation;
        rb.isKinematic = true; // hentikan kontrol player
    }
}
