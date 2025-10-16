using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    [Header("UI Panel dan Slot")]
    public GameObject inventoryPanel;
    public GameObject[] slotBorders; // border slot 1–5

    private int activeSlot = -1;
    private int lastClickedSlot = -1;
    private float lastClickTime;
    private float doubleClickDelay = 0.3f;

    void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);

        ClearHighlights();
    }

    void Update()
    {
        if (slotBorders == null || slotBorders.Length == 0) return;

        // Keyboard input (1–5)
        for (int i = 0; i < slotBorders.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                HandleSlotInput(i);
        }
    }

    void HandleSlotInput(int index)
    {
        if (InventoryManager.instance == null) return;

        bool isDouble = InventoryManager.instance.IsDoublePress(index);

        if (isDouble && activeSlot == index)
        {
            slotBorders[index]?.SetActive(false);
            activeSlot = -1;
            InventoryManager.instance.SetActiveSlot(-1);

            // Unequip flashlight jika ada
            FlashlightItem.instance?.OnSlotDoubleClicked(index);

            Debug.Log("Item dilepas dari slot " + (index + 1));
            return;
        }

        ForceHighlightSlot(index);
    }

    public void OnSlotClicked(int slotIndex)
    {
        float currentTime = Time.time;

        if (slotIndex == lastClickedSlot && (currentTime - lastClickTime) <= doubleClickDelay)
        {
            FlashlightItem.instance?.OnSlotDoubleClicked(slotIndex);
            lastClickTime = 0f;
            lastClickedSlot = -1;
            return;
        }

        ForceHighlightSlot(slotIndex);
        lastClickedSlot = slotIndex;
        lastClickTime = currentTime;
    }

    public void ForceHighlightSlot(int index)
    {
        if (slotBorders == null || InventoryManager.instance == null) return;

        for (int i = 0; i < slotBorders.Length; i++)
            slotBorders[i]?.SetActive(i == index);

        activeSlot = index;
        InventoryManager.instance.SetActiveSlot(index);

        // Cek apakah item di slot = flashlight
        if (InventoryManager.instance.GetItemAtSlot(index) == FlashlightItem.instance?.icon)
        {
            FlashlightItem.instance?.EquipFlashlight();
        }
        else
        {
            FlashlightItem.instance?.UnequipFlashlight();
        }

        Debug.Log("📦 Slot " + (index + 1) + " aktif.");
    }

    public void ClearHighlights()
    {
        if (slotBorders == null) return;

        foreach (var border in slotBorders)
            if (border != null)
                border.SetActive(false);

        activeSlot = -1;
        InventoryManager.instance?.SetActiveSlot(-1);
        FlashlightItem.instance?.UnequipFlashlight();
    }
}
