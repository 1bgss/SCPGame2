using UnityEngine;
using System.Collections;

public class ExitDoorController : MonoBehaviour
{
    [Header("Mode Pintu")]
    public bool isSlidingDoor = false;

    [Header("Swing Door Settings")]
    public float openAngle = 90f;
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
        // Simpan posisi dan rotasi awal
        closedRot = transform.rotation;
        closedPos = transform.position;

        openRot = closedRot * Quaternion.Euler(0, openAngle, 0);
        openPos = closedPos + slideOffset;
    }

    /// <summary>
    /// Dipanggil oleh TapExitCard ketika player berhasil tap kartu keluar.
    /// </summary>
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
                transform.position = Vector3.Lerp(closedPos, openPos, t);
                yield return null;
            }
        }
        else
        {
            while (t < 1f)
            {
                t += Time.deltaTime * openSpeed;
                transform.rotation = Quaternion.Slerp(closedRot, openRot, t);
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
                transform.position = Vector3.Lerp(openPos, closedPos, t);
                yield return null;
            }
        }
        else
        {
            while (t < 1f)
            {
                t += Time.deltaTime * openSpeed;
                transform.rotation = Quaternion.Slerp(openRot, closedRot, t);
                yield return null;
            }
        }

        isOpen = false;
    }
}
