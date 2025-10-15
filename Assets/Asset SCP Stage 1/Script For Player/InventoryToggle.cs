using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject[] slotBorders; // isi 5 border slot di Inspector (urutan 1–5)
    private int activeSlot = -1; // -1 artinya belum ada slot aktif

    void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);

        // Matikan semua border di awal
        foreach (GameObject border in slotBorders)
        {
            if (border != null)
                border.SetActive(false);
        }
    }

    void Update()
    {
        // Cek tombol 1 sampai 5
        for (int i = 0; i < slotBorders.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ToggleSlot(i);
            }
        }
    }

    void ToggleSlot(int index)
    {
        // Kalau slot yang ditekan sama dengan yang aktif → matikan semuanya
        if (activeSlot == index)
        {
            slotBorders[index].SetActive(false);
            activeSlot = -1;
            return;
        }

        // Matikan semua border dulu
        for (int i = 0; i < slotBorders.Length; i++)
        {
            if (slotBorders[i] != null)
                slotBorders[i].SetActive(false);
        }

        // Aktifkan slot yang ditekan
        if (slotBorders[index] != null)
            slotBorders[index].SetActive(true);

        activeSlot = index;
    }
}
