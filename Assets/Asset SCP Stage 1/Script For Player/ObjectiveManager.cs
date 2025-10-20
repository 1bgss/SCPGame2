using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager instance;

    [Header("UI References")]
    public TMP_Text objectiveText;

    // Teks awal
    private string header = "Objective\n───────────────\n";
    private string currentObjective = "Search Exit Card in the room";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        UpdateObjectiveUI();
    }

    public void SetObjective(string newText)
    {
        if (objectiveText == null) return;

        currentObjective = newText;
        UpdateObjectiveUI();
    }

    public void AddObjective(string newText)
    {
        if (objectiveText == null) return;

        currentObjective += "\n" + newText;
        UpdateObjectiveUI();
    }

    public void HideObjective()
    {
        if (objectiveText != null)
            objectiveText.gameObject.SetActive(false);
    }

    private void UpdateObjectiveUI()
    {
        if (objectiveText != null)
        {
            objectiveText.fontSize = 25f; // 🔹 Ukuran font diset ke 25
            objectiveText.text = header + currentObjective;
            objectiveText.gameObject.SetActive(true);
        }
    }
}
