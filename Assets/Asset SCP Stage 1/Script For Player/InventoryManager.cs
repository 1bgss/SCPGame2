using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Inventory Slot Settings")]
    public Image[] slotIcons;           // 5 slot icon dari UI
    public Sprite emptySlotSprite;      // sprite kosong (drag di Inspector)
    public int activeSlot = -1;         // slot aktif sekarang

    private bool[] isSlotEmpty;         // 🟢 penanda slot kosong/terisi
    private float doublePressDelay = 0.3f;
    private float[] lastPressTimes = new float[5];

    void Awake()
    {
        instance = this;
        isSlotEmpty = new bool[slotIcons.Length];

        // Awalnya semua slot dianggap kosong
        for (int i = 0; i < isSlotEmpty.Length; i++)
        {
            isSlotEmpty[i] = true;
            if (slotIcons[i] != null)
                slotIcons[i].sprite = emptySlotSprite;
        }
    }

    // ======================
    // Tambah item ke slot kosong
    // ======================
    public bool AddItem(Sprite itemIcon)
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (isSlotEmpty[i])
            {
                slotIcons[i].sprite = itemIcon;
                isSlotEmpty[i] = false;
                Debug.Log("✅ Item ditambahkan ke slot " + (i + 1));
                return true; // sukses
            }
        }

        Debug.Log("❌ Inventory penuh!");
        return false; // gagal (semua slot penuh)
    }

    // ======================
    // Hapus item tertentu (misal saat dibuang)
    // ======================
    public void RemoveItem(Sprite itemIcon)
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            // Cari item yang sesuai dan masih dianggap terisi
            if (slotIcons[i].sprite == itemIcon && !isSlotEmpty[i])
            {
                slotIcons[i].sprite = emptySlotSprite;
                isSlotEmpty[i] = true;

                // kalau slot ini sedang aktif, reset
                if (activeSlot == i)
                    SetActiveSlot(-1);

                Debug.Log("🗑️ Item dihapus dari slot " + (i + 1));
                return;
            }
        }
    }

    // ======================
    // Ambil icon item dari slot aktif
    // ======================
    public Sprite GetActiveItem()
    {
        if (activeSlot >= 0 && activeSlot < slotIcons.Length)
            return slotIcons[activeSlot].sprite;

        return null;
    }

    // ======================
    // Set slot aktif (dari tombol angka)
    // ======================
    public void SetActiveSlot(int index)
    {
        activeSlot = index;
        Debug.Log(index == -1
            ? "Tidak ada slot aktif"
            : "🎯 Slot aktif: " + (index + 1));
    }

    // ======================
    // Double press handler (untuk toggle item aktif)
    // ======================
    public bool IsDoublePress(int index)
    {
        if (Time.time - lastPressTimes[index] <= doublePressDelay)
        {
            lastPressTimes[index] = 0f; // reset waktu
            return true;
        }

        lastPressTimes[index] = Time.time;
        return false;
    }

    // ======================
    // Cek apakah slot aktif kosong
    // ======================
    public bool IsActiveSlotEmpty()
    {
        if (activeSlot < 0) return true;
        return isSlotEmpty[activeSlot];
    }

    // ======================
    // Utility tambahan (kalau nanti mau dicek dari luar)
    // ======================
    public bool IsInventoryFull()
    {
        foreach (bool empty in isSlotEmpty)
        {
            if (empty) return false; // masih ada slot kosong
        }
        return true; // semua penuh
    }
}
