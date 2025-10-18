using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SpiritIconClick : MonoBehaviour, IPointerClickHandler
{
    public string sceneToLoad = "NextScene";

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("🌀 Spirit Icon diklik dari UI!");
        SceneManager.LoadScene(sceneToLoad);
    }
}
