using UnityEngine;
using UnityEngine.AI;

public class DoorNavMeshObstacle : MonoBehaviour
{
    private NavMeshObstacle obstacle;

    void Start()
    {
        obstacle = GetComponent<NavMeshObstacle>();
    }

    public void SetObstacleActive(bool active)
    {
        obstacle.enabled = active;
    }
}
