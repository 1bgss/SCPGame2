using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject[] slotBorders; // border slot 1–5
    private int activeSlot = -1;

    void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);

        ClearHighlights();
    }

    void Update()
    {
        for (int i = 0; i < slotBorders.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                HandleSlotInput(i);
            }
        }
    }

    void HandleSlotInput(int index)
    {
        // cek double press
        bool isDouble = InventoryManager.instance.IsDoublePress(index);

        // kalau double press → lepas item
        if (isDouble && activeSlot == index)
        {
            slotBorders[index].SetActive(false);
            activeSlot = -1;
            InventoryManager.instance.SetActiveSlot(-1);
            Debug.Log("Item dilepas dari slot " + (index + 1));
            return;
        }

        ForceHighlightSlot(index);
    }

    // ==================================================
    // Fungsi tambahan dari kode sebelumnya
    // ==================================================
    public void ForceHighlightSlot(int index)
    {
        for (int i = 0; i < slotBorders.Length; i++)
        {
            if (slotBorders[i] != null)
                slotBorders[i].SetActive(i == index);
        }

        activeSlot = index;
        InventoryManager.instance.SetActiveSlot(index);
        Debug.Log("📦 Slot " + (index + 1) + " dipaksa aktif (otomatis oleh item).");
    }

    public void ClearHighlights()
    {
        foreach (var border in slotBorders)
            if (border != null)
                border.SetActive(false);

        activeSlot = -1;
        InventoryManager.instance.SetActiveSlot(-1);
    }
}
