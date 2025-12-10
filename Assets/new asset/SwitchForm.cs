using UnityEngine;

public class SwitchForm : MonoBehaviour
{
    public CapsuleCollider col;

    // ukuran collider untuk manusia
    public float humanHeight = 1.8f;
    public float humanRadius = 0.3f;
    public float humanYCenter = 0.9f;

    // ukuran collider untuk monster (lebih besar + bungkuk)
    public float monsterHeight = 1.4f;
    public float monsterRadius = 0.55f;
    public float monsterYCenter = 0.7f;

    bool isMonster = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isMonster = !isMonster;
            ApplyColliderChange();
        }
    }

    void ApplyColliderChange()
    {
        if (isMonster)
        {
            col.height = monsterHeight;
            col.radius = monsterRadius;
            col.center = new Vector3(0, monsterYCenter, 0);
            Debug.Log("SWITCHED → MONSTER MODE");
        }
        else
        {
            col.height = humanHeight;
            col.radius = humanRadius;
            col.center = new Vector3(0, humanYCenter, 0);
            Debug.Log("SWITCHED → HUMAN MODE");
        }
    }
}
