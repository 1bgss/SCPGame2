using UnityEngine;

public class GunValorant : MonoBehaviour
{
    [Header("Gun Stats")]
    public int maxAmmo = 30;
    public float fireRate = 0.09f;
    public float damage = 20f;
    public float range = 100f;
    public bool automatic = true;

    [Header("Accuracy")]
    public float baseSpread = 0.01f;
    public float movingSpread = 0.06f;
    public float adsSpread = 0.003f;

    [Header("Recoil")]
    public float recoilUp = 0.8f;
    public float recoilSide = 0.45f;
    public float adsRecoilMultiplier = 0.5f;

    [Header("ADS Settings")]
    public bool hasADS = true;
    public float adsFOV = 45f;
    public float normalFOV = 60f;
    public float adsSpeed = 10f;

    [Header("Projectile Mode")]
    public bool useProjectile = true;
    public GameObject bulletPrefab;
    public Transform firePoint; // ujung barrel

    [Header("Effects")]
    public Camera cam;
    public ParticleSystem muzzleFlash;
    public GameObject bulletHole;
    public LayerMask hitLayer;

    private int ammo;
    private bool canShoot = true;
    private float recoilY = 0f;
    private float recoilX = 0f;
    private CharacterController controller;

    void Start()
    {
        ammo = maxAmmo;
        controller = GetComponentInParent<CharacterController>();
        if (cam != null)
            cam.fieldOfView = normalFOV;
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && automatic && canShoot && ammo > 0)
            Shoot();

        if (Input.GetButtonDown("Fire1") && !automatic && canShoot && ammo > 0)
            Shoot();

        if (hasADS)
            HandleADS();
    }

    // ===================== ADS =====================
    void HandleADS()
    {
        if (cam == null) return;

        if (Input.GetButton("Fire2"))
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, adsFOV, Time.deltaTime * adsSpeed);
        else
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normalFOV, Time.deltaTime * adsSpeed);
    }

    // ===================== SHOOT =====================
    void Shoot()
    {
        canShoot = false;
        ammo--;

        if (muzzleFlash) muzzleFlash.Play();

        float spread = CalculateSpread();

        // direction spread
        Vector3 dir = cam.transform.forward
                      + cam.transform.right * Random.Range(-spread, spread)
                      + cam.transform.up * Random.Range(-spread, spread);

        // ================= Projectile Mode =================
        if (useProjectile && bulletPrefab != null && firePoint != null)
        {
            // paksa rotation sesuai camera + spread
            Quaternion bulletRot = Quaternion.LookRotation(dir);
            GameObject b = Instantiate(bulletPrefab, firePoint.position, bulletRot);

            Bullet blt = b.GetComponent<Bullet>();
            if (blt != null) blt.damage = damage;
        }
        else // ================= Raycast Mode =================
        {
            if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, range, hitLayer))
            {
                if (bulletHole != null)
                {
                    GameObject h = Instantiate(bulletHole, hit.point + hit.normal * 0.01f,
                                               Quaternion.LookRotation(hit.normal));
                    Destroy(h, 7f);
                }
            }
        }

        AddRecoil();
        Invoke(nameof(ResetShoot), fireRate);
    }

    float CalculateSpread()
    {
        if (Input.GetButton("Fire2") && hasADS) return adsSpread;
        return controller && controller.velocity.magnitude > 0.2f ? movingSpread : baseSpread;
    }

    void ResetShoot() => canShoot = true;

    // ===================== RECOIL =====================
    void AddRecoil()
    {
        float mul = (Input.GetButton("Fire2") && hasADS) ? adsRecoilMultiplier : 1f;

        recoilY += recoilUp * mul;
        recoilX += Random.Range(-recoilSide, recoilSide) * mul;

        cam.transform.localRotation = Quaternion.Euler(recoilY, recoilX, 0);
    }

    public void ResetRecoil()
    {
        recoilY = 0;
        recoilX = 0;
        if (cam != null)
            cam.transform.localRotation = Quaternion.identity;
    }

    // ===================== RELOAD =====================
    public void Reload()
    {
        ammo = maxAmmo;
        ResetRecoil();
    }
}
