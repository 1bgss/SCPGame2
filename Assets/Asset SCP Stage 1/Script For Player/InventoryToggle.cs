using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel; // drag panel kamu ke sini di Inspector

    void Start()
    {
        // Inventory selalu aktif sejak awal
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);
    }

    void Update()
    {
        // Tidak ada input toggle — inventory selalu terlihat
    }
}
