using UnityEngine;

public class DoubleDoorControllerAutoPivot : MonoBehaviour
{
    [Header("Assign door objects")]
    public Transform leftDoor;   // objek pintu kiri
    public Transform rightDoor;  // objek pintu kanan

    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public float doorWidth = 1f; // lebar pintu untuk offset pivot
    public bool isOpen = false;

    private Quaternion leftClosedRot, rightClosedRot;
    private Quaternion leftOpenRot, rightOpenRot;
    private Vector3 leftDefaultPos, rightDefaultPos;

    void Start()
    {
        // Simpan posisi dan rotasi awal
        leftClosedRot = leftDoor.rotation;
        rightClosedRot = rightDoor.rotation;
        leftDefaultPos = leftDoor.position;
        rightDefaultPos = rightDoor.position;

        // Hitung posisi pivot virtual (geser sedikit agar seperti engsel)
        leftDoor.position -= leftDoor.right * (doorWidth / 2f);
        rightDoor.position += rightDoor.right * (doorWidth / 2f);

        // Rotasi target buka
        leftOpenRot = leftClosedRot * Quaternion.Euler(0, -openAngle, 0);
        rightOpenRot = rightClosedRot * Quaternion.Euler(0, openAngle, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        // Interpolasi rotasi pintu kiri & kanan
        if (isOpen)
        {
            leftDoor.rotation = Quaternion.Slerp(leftDoor.rotation, leftOpenRot, Time.deltaTime * openSpeed);
            rightDoor.rotation = Quaternion.Slerp(rightDoor.rotation, rightOpenRot, Time.deltaTime * openSpeed);
        }
        else
        {
            leftDoor.rotation = Quaternion.Slerp(leftDoor.rotation, leftClosedRot, Time.deltaTime * openSpeed);
            rightDoor.rotation = Quaternion.Slerp(rightDoor.rotation, rightClosedRot, Time.deltaTime * openSpeed);
        }
    }

    void OnDisable()
    {
        // Reset posisi biar gak geser permanen di editor
        leftDoor.position = leftDefaultPos;
        rightDoor.position = rightDefaultPos;
    }
}
