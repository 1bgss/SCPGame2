using UnityEngine;
using System.Collections;

public class PotionMain : MonoBehaviour
{
    public static PotionMain instance; // << ini wajib

    public PlayerStamina playerStamina;
    public GameObject playerPotionObject;
    public Sprite icon;
    public float effectDuration = 10f;

    private bool isActive = false;
    private bool isEffectActive = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isActive)
        {
            UsePotion();
        }
    }

    public void UsePotion()
    {
        if (playerStamina == null || isEffectActive) return;

        isActive = true;
        isEffectActive = true;

        if (playerPotionObject != null)
            playerPotionObject.SetActive(true);

        StartCoroutine(ApplyStaminaEffect());
    }

    private IEnumerator ApplyStaminaEffect()
    {
        float originalMax = playerStamina.maxStamina;
        float originalDrain = playerStamina.staminaDrain;

        playerStamina.maxStamina = 9999f;
        playerStamina.staminaDrain = 0f;
        Debug.Log("💚 Potion digunakan: stamina unlimited");

        yield return new WaitForSeconds(effectDuration);

        playerStamina.maxStamina = originalMax;
        playerStamina.staminaDrain = originalDrain;
        Debug.Log("💔 Potion efek habis");

        if (InventoryManager.instance != null)
        {
            int slotIndex = InventoryManager.instance.activeSlot;
            InventoryManager.instance.ClearSlot(slotIndex);
            InventoryManager.instance.SetActiveSlot(-1);
        }

        if (playerPotionObject != null)
            playerPotionObject.SetActive(false);

        isEffectActive = false;
        isActive = false;

        Destroy(gameObject);
    }
}
