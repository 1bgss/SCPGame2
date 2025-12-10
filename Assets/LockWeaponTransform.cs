using UnityEngine;

public class LockWeaponTransform : MonoBehaviour
{
    void LateUpdate()
    {
        transform.localPosition = new Vector3(0f, 102.639f, 0f);
        transform.localRotation = Quaternion.Euler(-1.12f, -0.957f, 1.39f);
    }
}
