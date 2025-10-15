using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public Light flashlightLight; // Drag Spot Light di prefab
    private bool isOn = true;

    void Start()
    {
        if (flashlightLight != null)
            flashlightLight.enabled = isOn; // awal nyala
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // Tekan F untuk toggle
        {
            isOn = !isOn;
            if (flashlightLight != null)
                flashlightLight.enabled = isOn;
        }
    }
}
