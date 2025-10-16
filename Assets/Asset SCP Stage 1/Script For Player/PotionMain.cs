using UnityEngine;
using System.Collections;

public class PotionMain : MonoBehaviour
{
    [Header("Referensi Player & Stamina")]
    public PlayerStamina playerStamina;
    public float effectDuration = 10f; // durasi stamina unlimited
    private bool isActive = false;

    // Referensi tambahan
    public GameObject playerPotionObject; // object potion di tangan player
    private bool isHeld = false;
    private bool isEffectActive = false;

    void Update()
    {
        // Tekan F untuk minum potion
        if (Input.GetKeyDown(KeyCode.F) && !isActive)
        {
            UsePotion();
        }
    }

    public void UsePotion()
    {
        if (playerStamina == null) return;

        isActive = true;
        isEffectActive = true;
        StartCoroutine(ApplyStaminaEffect());
    }

    private IEnumerator ApplyStaminaEffect()
    {
        float originalMax = playerStamina.maxStamina;
        float originalDrain = playerStamina.staminaDrain;

        // Efek stamina unlimited
        playerStamina.maxStamina = 9999f;
        playerStamina.staminaDrain = 0f;
        Debug.Log("💚 Potion digunakan: stamina unlimited 10 detik");

        yield return new WaitForSeconds(effectDuration);

        // Reset stamina ke normal
        playerStamina.maxStamina = originalMax;
        playerStamina.staminaDrain = originalDrain;
        Debug.Log("💔 Potion efek habis");

        // 🔻 Hapus dari inventory dan nonaktifkan object potion
        int slotIndex = InventoryManager.instance.activeSlot;
        InventoryManager.instance.ClearSlot(slotIndex);
        InventoryManager.instance.SetActiveSlot(-1);

        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);

        isHeld = false;
        isEffectActive = false;
        isActive = false;

        // Hapus object potion dari scene
        Destroy(gameObject);
    }
}
