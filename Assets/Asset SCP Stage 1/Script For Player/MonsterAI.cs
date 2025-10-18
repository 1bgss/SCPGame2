using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float waypointStopDistance = 0.3f;
    public float waitAtWaypoint = 1f;

    [Header("Movement Speeds")]
    public float patrolSpeed = 1.2f;
    public float chaseSpeed = 3.5f;

    [Header("Detection Settings")]
    public Transform player;
    public float viewRadius = 8f;
    [Range(0, 360)]
    public float viewAngle = 60f;
    public LayerMask playerLayer;
    public LayerMask obstacleMask;
    public float timeToLosePlayer = 3f;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    [Header("Scene Transition")]
    public string youDiedScene = "You died";
    public float fadeDelay = 0.5f;

    [Header("Animation (Optional)")]
    public Animator animator;

    private NavMeshAgent agent;
    private int currentWaypoint = 0;
    private float waitTimer = 0f;
    private bool playerInSight = false;
    private float playerLostTimer = 0f;
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        if (waypoints.Length > 0)
            agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        if (player == null) return;

        if (CanSeePlayer())
        {
            playerInSight = true;
            playerLostTimer = 0f;
        }
        else if (playerInSight)
        {
            playerLostTimer += Time.deltaTime;
            if (playerLostTimer >= timeToLosePlayer)
            {
                playerInSight = false;
                playerLostTimer = 0f;
            }
        }

        if (playerInSight)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        if (Vector3.Distance(transform.position, player.position) <= viewRadius)
        {
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, viewRadius, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        agent.speed = patrolSpeed;
        if (!agent.pathPending && agent.remainingDistance < waypointStopDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitAtWaypoint)
            {
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                agent.SetDestination(waypoints[currentWaypoint].position);
                waitTimer = 0f;
            }
        }

        if (animator) animator.SetBool("isChasing", false);
    }

    void ChasePlayer()
    {
        if (player == null) return;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        if (animator) animator.SetBool("isChasing", true);

        if (!isAttacking && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            StartCoroutine(DoAttack());
        }
    }

    System.Collections.IEnumerator DoAttack()
    {
        isAttacking = true;
        if (animator) animator.SetTrigger("attack");

        yield return new WaitForSeconds(0.3f); // sedikit delay biar lebih natural

        if (Vector3.Distance(transform.position, player.position) <= attackRange + 0.5f)
        {
            Debug.Log("💀 SCP menangkap player! Game Over!");

            PlayerDeathEffect death = player.GetComponent<PlayerDeathEffect>();
            if (death != null)
            {
                death.Die(); // langsung panggil mati
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    }

    Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
