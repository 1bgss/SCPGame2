using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Referensi Slot UI")]
    public Image[] slotIcons; // icon tiap slot
    public int activeSlot = -1;

    private Sprite[] storedItems;
    private float[] lastPressTimes;
    private float doublePressDelay = 0.3f;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        storedItems = new Sprite[slotIcons.Length];
        lastPressTimes = new float[slotIcons.Length];
    }

    // ✅ Tambah item ke slot kosong
    public bool AddItem(Sprite icon)
    {
        for (int i = 0; i < storedItems.Length; i++)
        {
            if (storedItems[i] == null)
            {
                storedItems[i] = icon;
                slotIcons[i].sprite = icon;
                slotIcons[i].enabled = true;
                Debug.Log($"📦 Item ditambahkan ke slot {i + 1}");
                return true;
            }
        }
        Debug.Log("❌ Inventory penuh!");
        return false;
    }

    // ✅ Hapus item (panggil ClearSlot agar konsisten)
    public void RemoveItem(Sprite icon)
    {
        for (int i = 0; i < storedItems.Length; i++)
        {
            if (storedItems[i] == icon)
            {
                ClearSlot(i);
                return;
            }
        }
    }

    // ✅ Set slot aktif
    public void SetActiveSlot(int index)
    {
        activeSlot = index;
        Debug.Log($"🎯 Slot aktif sekarang: {index + 1}");
    }

    // ✅ Cek klik ganda
    public bool IsDoublePress(int index)
    {
        float time = Time.time;
        if (time - lastPressTimes[index] <= doublePressDelay)
        {
            lastPressTimes[index] = 0f;
            return true;
        }
        lastPressTimes[index] = time;
        return false;
    }

    // ✅ Ambil item dari slot tertentu
    public Sprite GetItemAtSlot(int index)
    {
        if (index >= 0 && index < storedItems.Length)
            return storedItems[index];
        return null;
    }

    // ✅ Kosongkan semua slot
    public void ClearAllSlots()
    {
        for (int i = 0; i < storedItems.Length; i++)
        {
            storedItems[i] = null;
            slotIcons[i].sprite = null;
            slotIcons[i].enabled = false;
        }
        activeSlot = -1;
        Debug.Log("🧹 Semua slot dikosongkan.");
    }

    // ✅ Kosongkan satu slot tertentu
    public void ClearSlot(int index)
    {
        if (index < 0 || index >= storedItems.Length) return;

        storedItems[index] = null;
        slotIcons[index].sprite = null;
        slotIcons[index].enabled = false;
        Debug.Log($"🗑️ Slot {index + 1} dikosongkan.");
    }
}
