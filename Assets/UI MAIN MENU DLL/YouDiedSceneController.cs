using UnityEngine;
using UnityEngine.SceneManagement;

public class YouDiedSceneController : MonoBehaviour
{
    [Header("Scene Settings")]
    public string mainMenuScene = "MainMenu";
    public float fadeDuration = 1.0f;   // durasi fade in/out
    public float stayDuration = 2.5f;   // waktu sebelum balik ke main menu

    private Texture2D fadeTexture;
    private float alpha = 1f;
    private int fadeDir = -1; // -1 = fade in, 1 = fade out

    void Start()
    {
        // Buat tekstur hitam fullscreen untuk fade
        fadeTexture = new Texture2D(1, 1);
        fadeTexture.SetPixel(0, 0, Color.black);
        fadeTexture.Apply();

        // Mulai fade-in (muncul dari mati)
        StartCoroutine(FadeSequence());
    }

    private System.Collections.IEnumerator FadeSequence()
    {
        // Fade in (dari hitam ke terang)
        yield return StartCoroutine(Fade(-1));

        // Tahan beberapa detik di layar “You Died”
        yield return new WaitForSeconds(stayDuration);

        // Fade out (gelap lagi)
        yield return StartCoroutine(Fade(1));

        // Ganti scene ke Main Menu
        SceneManager.LoadScene(mainMenuScene);
    }

    private System.Collections.IEnumerator Fade(int direction)
    {
        fadeDir = direction;
        float fadeSpeed = 1f / fadeDuration;
        while ((direction == -1 && alpha > 0f) || (direction == 1 && alpha < 1f))
        {
            alpha += direction * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);
            yield return null;
        }
    }

    void OnGUI()
    {
        // Gambar layer hitam di atas layar (fade)
        if (fadeTexture == null) return;
        GUI.color = new Color(0, 0, 0, alpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
    }
}
