using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Referensi Slot UI")]
    public Image[] slotIcons; // icon tiap slot
    public int activeSlot = -1;

    private Sprite[] storedItems;
    private MonoBehaviour[] storedItemObjects; // reference object per slot
    private float[] lastPressTimes;
    private float doublePressDelay = 0.3f;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        storedItems = new Sprite[slotIcons.Length];
        storedItemObjects = new MonoBehaviour[slotIcons.Length];
        lastPressTimes = new float[slotIcons.Length];
    }

    // =====================
    // Tambah item ke slot kosong (sprite + reference object)
    // =====================
    public bool AddItem(Sprite icon, MonoBehaviour itemObject)
    {
        for (int i = 0; i < storedItems.Length; i++)
        {
            if (storedItems[i] == null)
            {
                storedItems[i] = icon;
                storedItemObjects[i] = itemObject;

                if (slotIcons[i] != null)
                {
                    slotIcons[i].sprite = icon;
                    slotIcons[i].enabled = true;
                }

                // Set activeSlot otomatis kalau belum ada
                if (activeSlot == -1)
                    activeSlot = i;

                Debug.Log($"📦 Item ditambahkan ke slot {i + 1}");
                return true;
            }
        }
        Debug.Log("❌ Inventory penuh!");
        return false;
    }

    // =====================
    // Hapus item (gunakan ClearSlot agar konsisten)
    // =====================
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

    // =====================
    // Kosongkan satu slot tertentu
    // =====================
    public void ClearSlot(int index)
    {
        if (index < 0 || index >= storedItems.Length) return;

        storedItems[index] = null;
        storedItemObjects[index] = null;

        if (slotIcons[index] != null)
        {
            slotIcons[index].sprite = null;
            slotIcons[index].enabled = false;
        }

        // Reset activeSlot jika slot ini aktif
        if (activeSlot == index)
            activeSlot = -1;

        Debug.Log($"🗑️ Slot {index + 1} dikosongkan.");
    }

    // =====================
    // Kosongkan semua slot
    // =====================
    public void ClearAllSlots()
    {
        for (int i = 0; i < storedItems.Length; i++)
            ClearSlot(i);

        activeSlot = -1;
        Debug.Log("🧹 Semua slot dikosongkan.");
    }

    // =====================
    // Ambil item dari slot tertentu
    // =====================
    public Sprite GetItemAtSlot(int index)
    {
        if (index >= 0 && index < storedItems.Length)
            return storedItems[index];
        return null;
    }

    // =====================
    // Ambil reference object dari slot tertentu
    // =====================
    public MonoBehaviour GetItemObjectAtSlot(int index)
    {
        if (index >= 0 && index < storedItemObjects.Length)
            return storedItemObjects[index];
        return null;
    }

    // =====================
    // Set slot aktif
    // =====================
    public void SetActiveSlot(int index)
    {
        if (index < -1 || index >= storedItems.Length) return;
        activeSlot = index;
        Debug.Log($"🎯 Slot aktif sekarang: {index + 1}");
    }

    // =====================
    // Cek klik ganda (double-press)
    // =====================
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
}
