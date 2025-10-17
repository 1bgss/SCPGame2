using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TapExitCard : MonoBehaviour
{
    [Header("Referensi")]
    public GameObject greenLight;
    public GameObject redLight;
    public Animator doorAnimator;
    public string blackScreenSceneName = "BlackScreenScene";
    public CanvasGroup fadeCanvas; // CanvasGroup untuk fade
    public float fadeDuration = 1f;
    public GameObject toBeContinuedUI; // UI Panel dengan teks & tombol

    private bool canTap = false;
    private bool doorOpened = false;

    void Update()
    {
        if (!canTap) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            bool hasExitCard = CheckExitCardInInventory();

            if (hasExitCard)
            {
                greenLight.SetActive(true);
                redLight.SetActive(false);

                if (!doorOpened)
                {
                    doorAnimator.SetTrigger("Open");
                    doorOpened = true;
                }
            }
            else
            {
                redLight.SetActive(true);
                greenLight.SetActive(false);
            }
        }
    }

    private bool CheckExitCardInInventory()
    {
        if (InventoryManager.instance == null) return false;

        for (int i = 0; i < InventoryManager.instance.slotIcons.Length; i++)
        {
            var obj = InventoryManager.instance.GetItemObjectAtSlot(i);
            if (obj != null && obj is ExitCardItem)
                return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTap = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTap = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!doorOpened) return;
        if (other.CompareTag("Player"))
        {
            doorAnimator.SetTrigger("Close"); // Tutup pintu saat player masuk
            doorOpened = false;

            // Mulai transisi ke black screen
            StartCoroutine(FadeToBlack());
        }
    }

    private IEnumerator FadeToBlack()
    {
        if (fadeCanvas != null)
        {
            fadeCanvas.gameObject.SetActive(true);
            fadeCanvas.alpha = 0;

            float t = 0;
            while (t < fadeDuration)
            {
                fadeCanvas.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
                t += Time.deltaTime;
                yield return null;
            }
            fadeCanvas.alpha = 1;
        }

        if (toBeContinuedUI != null)
            toBeContinuedUI.SetActive(true);

        // Bisa pause game saat muncul To Be Continued
        Time.timeScale = 0f;
    }

    // Fungsi tombol Back to Main Menu
    public void BackToMainMenu(string mainMenuScene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }
}
