using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 5f;        // stamina penuh
    public float staminaDrain = 1f;      // stamina habis per detik lari
    public float staminaRegen = 0.5f;    // stamina balik per detik kalau ga lari
    private float currentStamina;

    [Header("UI Components")]
    public Slider staminaSlider;         // drag Slider dari Canvas
    public Text staminaText;             // drag Text dari Canvas

    void Start()
    {
        currentStamina = maxStamina;

        // setup slider
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
    }

    void Update()
    {
        // update UI setiap frame
        if (staminaSlider != null)
            staminaSlider.value = currentStamina;

        if (staminaText != null)
            staminaText.text = "STAMINA: " + Mathf.Round(currentStamina);
    }

    // --- Dipanggil dari PlayerMovement ketika lari ---
    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0f) currentStamina = 0f;
    }

    // --- Dipanggil dari PlayerMovement ketika jalan/diam ---
    public void RegenStamina(float amount)
    {
        currentStamina += amount;
        if (currentStamina > maxStamina) currentStamina = maxStamina;
    }

    // --- Getter buat akses stamina player ---
    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public bool HasStamina()
    {
        return currentStamina > 0f;
    }
}
