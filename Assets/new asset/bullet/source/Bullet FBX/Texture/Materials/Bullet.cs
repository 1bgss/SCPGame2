using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 120f;
    public float damage = 20f;
    public float lifeTime = 4f;
    public LayerMask hitLayer;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;              // peluru cepat = tanpa gravitasi
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision col)
    {
        // Bisa tambah damage kalau ada script enemy HP
        // EnemyHP hp = col.collider.GetComponent<EnemyHP>();
        // if (hp) hp.TakeDamage(damage);

        Destroy(gameObject);
    }
}
