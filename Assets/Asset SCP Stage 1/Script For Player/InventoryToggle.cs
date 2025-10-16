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
            HideAllMainObjects();
            slotBorders[index]?.SetActive(false);
            activeSlot = -1;
            InventoryManager.instance.SetActiveSlot(-1);

            Debug.Log("Item dilepas dari slot " + (index + 1));
            return;
        }

        ForceHighlightSlot(index);
    }

    public void OnSlotClicked(int slotIndex)
    {
        float currentTime = Time.time;

        bool isDouble = (slotIndex == lastClickedSlot && (currentTime - lastClickTime) <= doubleClickDelay);

        if (isDouble)
        {
            HideAllMainObjects();
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

        // highlight slot
        for (int i = 0; i < slotBorders.Length; i++)
            slotBorders[i]?.SetActive(i == index);

        activeSlot = index;
        InventoryManager.instance.SetActiveSlot(index);

        // hide semua main object dulu
        HideAllMainObjects();

        // tampilkan main object berdasarkan slot
        var item = InventoryManager.instance.GetItemAtSlot(index);
        if (item != null)
        {
            if (item == FlashlightItem.instance?.icon)
                FlashlightItem.instance?.EquipFlashlight();
            else if (item == PotionItem.instance?.icon)
                PotionItem.instance?.playerPotionObject?.SetActive(true);
            else if (item == PotionRunningItem.instance?.icon)
                PotionRunningItem.instance?.playerPotionObject?.SetActive(true); // Speed Potion
            else if (item == StaminaPotionItem.instance?.icon)
                StaminaPotionItem.instance?.playerPotionObject?.SetActive(true); // Stamina Potion
        }

        Debug.Log("📦 Slot " + (index + 1) + " aktif.");
    }

    void HideAllMainObjects()
    {
        FlashlightItem.instance?.UnequipFlashlight();

        if (PotionItem.instance?.playerPotionObject != null)
            PotionItem.instance.playerPotionObject.SetActive(false);

        if (PotionRunningItem.instance?.playerPotionObject != null)
            PotionRunningItem.instance.playerPotionObject.SetActive(false); // Speed Potion

        if (StaminaPotionItem.instance?.playerPotionObject != null)
            StaminaPotionItem.instance.playerPotionObject.SetActive(false); // Stamina Potion
    }

    public void ClearHighlights()
    {
        if (slotBorders == null) return;

        foreach (var border in slotBorders)
            border?.SetActive(false);

        activeSlot = -1;
        InventoryManager.instance?.SetActiveSlot(-1);

        HideAllMainObjects();
    }
}
