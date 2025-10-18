using UnityEngine;
using UnityEngine.SceneManagement;

public class SpiritClickHandler : MonoBehaviour
{
    public string sceneToLoad = "NextScene";
    private bool isLoading = false;

    void OnMouseDown()
    {
        if (isLoading) return;
        isLoading = true;

        Debug.Log("✨ Spirit Icon diklik!");
        SceneManager.LoadScene(sceneToLoad);
    }
}
