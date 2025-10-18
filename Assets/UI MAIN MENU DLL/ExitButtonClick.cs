using UnityEngine;
using UnityEngine.EventSystems;

public class ExitButtonClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("❌ Tombol Exit diklik!");
        ExitGame();
    }

    void ExitGame()
    {
#if UNITY_EDITOR
        // Kalau lagi di Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Kalau sudah di-build jadi game
        Application.Quit();
#endif
    }
}
