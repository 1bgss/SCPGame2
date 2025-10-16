using UnityEngine;

public interface IInventoryItem
{
    Sprite GetIcon();
    void OnEquip();
    void OnUnequip();
    void OnDrop();
}
