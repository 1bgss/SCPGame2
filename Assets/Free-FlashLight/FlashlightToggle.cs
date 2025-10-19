using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public Light flashlightLight; // Drag Spot Light di prefab
    private bool isOn = true;

    [Header("Audio")]
    public AudioSource audioSource;    // AudioSource untuk suara toggle
    public AudioClip toggleClip;       // Clip suara nyalain/matiin

    void Start()
    {
        if (flashlightLight != null)
            flashlightLight.enabled = isOn; // awal nyala

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // Tekan F untuk toggle
        {
            isOn = !isOn;

            if (flashlightLight != null)
                flashlightLight.enabled = isOn;

            PlayToggleSound();
        }
    }

    void PlayToggleSound()
    {
        if (audioSource != null && toggleClip != null)
        {
            audioSource.PlayOneShot(toggleClip, 1f); // main di atas clip lain
        }
    }
}
