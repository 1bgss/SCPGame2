using UnityEngine;

public class InventoryAlwaysVisible : MonoBehaviour
{
    [Header("Inventory Panel")]
    public GameObject inventoryPanel; // drag panel inventory ke sini di Inspector

    void Start()
    {
        // Pastikan panel selalu aktif dari awal
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);
    }

    void Update()
    {
        // Tidak ada input toggle — inventory selalu terlihat
    }
}
