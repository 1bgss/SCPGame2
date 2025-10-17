using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        // Ganti "SceneX" dengan nama scene gameplay kamu
        SceneManager.LoadScene("SceneX");
    }

    public void OpenTutorial()
    {
        Debug.Log("Tutorial belum diaktifkan.");
        // Nanti bisa buka panel tutorial atau pindah ke scene tutorial
    }

    public void ExitGame()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
