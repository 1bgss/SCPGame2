using UnityEngine;
using System.Collections;

public class PotionMain : MonoBehaviour
{
    [Header("Referensi Player & Stamina")]
    public PlayerStamina playerStamina;
    public float effectDuration = 10f; // durasi stamina unlimited
    private bool isActive = false;

    void Update()
    {
        // Cek F ditekan
        if (Input.GetKeyDown(KeyCode.F) && !isActive)
        {
            UsePotion();
        }
    }

    public void UsePotion()
    {
        if (playerStamina == null) return;

        isActive = true;
        StartCoroutine(ApplyStaminaEffect());
    }

    private IEnumerator ApplyStaminaEffect()
    {
        float originalMax = playerStamina.maxStamina;
        float originalDrain = playerStamina.staminaDrain;

        // Stamina unlimited
        playerStamina.maxStamina = 9999f;
        playerStamina.staminaDrain = 0f;
        Debug.Log("💚 Potion digunakan: stamina unlimited 10 detik");

        yield return new WaitForSeconds(effectDuration);

        // Reset stamina
        playerStamina.maxStamina = originalMax;
        playerStamina.staminaDrain = originalDrain;
        Debug.Log("💔 Potion efek habis");

        isActive = false;
    }
}
