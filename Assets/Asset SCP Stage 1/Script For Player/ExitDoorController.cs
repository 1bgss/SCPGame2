using UnityEngine;
using System.Collections;

public class ExitDoorController : MonoBehaviour
{
    [Header("Mode Pintu")]
    public bool isSlidingDoor = false;

    [Header("Swing Door Settings")]
    public float openAngle = 90f;
    public bool invertRotation = false; // 🔹 Bisa ubah arah bukaan
    public float openSpeed = 2f;

    [Header("Sliding Door Settings")]
    public Vector3 slideOffset = new Vector3(0, 0, 2f);
    public float slideSpeed = 2f;

    [Header("Waktu Pintu Terbuka (detik)")]
    public float holdOpenTime = 2f;

    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;
    private Vector3 closedPos;
    private Vector3 openPos;

    void Start()
    {
        // Simpan posisi & rotasi awal (pakai local biar tetap benar kalau parent-nya dipindah)
        closedRot = transform.localRotation;
        closedPos = transform.localPosition;

        // Hitung rotasi target (arah buka bisa dibalik)
        float finalAngle = invertRotation ? -openAngle : openAngle;
        openRot = closedRot * Quaternion.Euler(0, finalAngle, 0);

        // Posisi pintu kalau sliding
        openPos = closedPos + slideOffset;
    }

    public void OpenDoor()
    {
        if (!isOpen)
            StartCoroutine(OpenDoorCoroutine());
    }

    private IEnumerator OpenDoorCoroutine()
    {
        isOpen = true;
        float t = 0f;

        if (isSlidingDoor)
        {
            while (t < 1f)
            {
                t += Time.deltaTime * slideSpeed;
                transform.localPosition = Vector3.Lerp(closedPos, openPos, t);
                yield return null;
            }
        }
        else
        {
            while (t < 1f)
            {
                t += Time.deltaTime * openSpeed;
                transform.localRotation = Quaternion.Slerp(closedRot, openRot, t);
                yield return null;
            }
        }

        yield return new WaitForSeconds(holdOpenTime);
        StartCoroutine(CloseDoorCoroutine());
    }

    private IEnumerator CloseDoorCoroutine()
    {
        float t = 0f;

        if (isSlidingDoor)
        {
            while (t < 1f)
            {
                t += Time.deltaTime * slideSpeed;
                transform.localPosition = Vector3.Lerp(openPos, closedPos, t);
                yield return null;
            }
        }
        else
        {
            while (t < 1f)
            {
                t += Time.deltaTime * openSpeed;
                transform.localRotation = Quaternion.Slerp(openRot, closedRot, t);
                yield return null;
            }
        }

        isOpen = false;
    }
}
