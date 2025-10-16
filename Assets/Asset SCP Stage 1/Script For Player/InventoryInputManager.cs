using UnityEngine;

public class InventoryInputManager : MonoBehaviour
{
    void Update()
    {
        // Tombol F untuk pakai item aktif
        if (Input.GetKeyDown(KeyCode.F))
        {
            int activeSlot = InventoryManager.instance.activeSlot;
            if (activeSlot == -1) return;

            // Ambil reference item dari slot aktif
            var item = InventoryManager.instance.GetItemObjectAtSlot(activeSlot) as PotionSpeedItem;
            if (item != null)
            {
                item.UsePotion();
            }
        }
    }
}
