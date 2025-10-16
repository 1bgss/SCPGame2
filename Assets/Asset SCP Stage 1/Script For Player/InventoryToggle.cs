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

        // Highlight slot
        for (int i = 0; i < slotBorders.Length; i++)
            slotBorders[i]?.SetActive(i == index);

        activeSlot = index;
        InventoryManager.instance.SetActiveSlot(index);

        // Hide semua main object dulu
        HideAllMainObjects();

        // Tampilkan main object berdasarkan reference dari InventoryManager
        var itemObject = InventoryManager.instance.GetItemObjectAtSlot(index);
        if (itemObject != null)
        {
            // Potion stamina / unlimited
            var staminaPotion = itemObject as PotionItem;
            if (staminaPotion != null && staminaPotion.playerPotionObject != null)
            {
                staminaPotion.playerPotionObject.SetActive(true);
            }

            // Potion running / speed
            var speedPotion = itemObject as PotionRunningItem;
            if (speedPotion != null && speedPotion.playerPotionObject != null)
            {
                speedPotion.playerPotionObject.SetActive(true);
            }

            // Flashlight
            var flashlight = itemObject as FlashlightItem;
            if (flashlight != null)
            {
                flashlight.EquipFlashlight();
            }
        }

        Debug.Log("📦 Slot " + (index + 1) + " aktif.");
    }

    void HideAllMainObjects()
    {
        if (InventoryManager.instance == null) return;

        // Loop semua slot dan hide object jika masih ada
        for (int i = 0; i < InventoryManager.instance.slotIcons.Length; i++)
        {
            var obj = InventoryManager.instance.GetItemObjectAtSlot(i);
            if (obj == null) continue;

            // Potion stamina / unlimited
            var staminaPotion = obj as PotionItem;
            if (staminaPotion != null && staminaPotion.playerPotionObject != null)
                staminaPotion.playerPotionObject.SetActive(false);

            // Potion running / speed
            var speedPotion = obj as PotionRunningItem;
            if (speedPotion != null && speedPotion.playerPotionObject != null)
                speedPotion.playerPotionObject.SetActive(false);

            // Flashlight
            var flashlight = obj as FlashlightItem;
            if (flashlight != null)
                flashlight.UnequipFlashlight();
        }
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
