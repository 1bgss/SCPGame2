using UnityEngine;

public class ZombieCameraFollow : MonoBehaviour
{
    [Header("Target = Zombie Body")]
    public Transform zombieTarget;

    [Header("Offset Kamera dari Badan")]
    public Vector3 offset = new Vector3(0, 1.8f, -2.5f);

    [Header("Fixed Camera Rotation (Look Down Body)")]
    public Vector3 fixedRotation = new Vector3(21.821f, -0.992f, -0.204f);

    public float smoothSpeed = 8f;

    void LateUpdate()
    {
        if (zombieTarget == null) return;

        // ====== POSISI FOLLOW ======
        Vector3 desiredPos = zombieTarget.position
                            + zombieTarget.up * offset.y
                            + zombieTarget.forward * offset.z
                            + zombieTarget.right * offset.x;

        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        // ====== ROTASI DIKUNCI SESUAI ANGLE YANG KAMU MAU ======
        Quaternion desiredRot = Quaternion.Euler(fixedRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, smoothSpeed * Time.deltaTime);
    }
}
