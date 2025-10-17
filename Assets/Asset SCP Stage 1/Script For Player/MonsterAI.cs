using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement; // <-- penting untuk restart scene

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] waypoints;
    public float waypointStopDistance = 0.3f;
    public float waitAtWaypoint = 1.0f;

    [Header("Movement Speeds")]
    public float patrolSpeed = 1.2f;
    public float chaseSpeed = 3.5f;

    [Header("Detection")]
    public Transform player;
    public float viewRadius = 8f;
    [Range(0, 360)] public float viewAngle = 60f;
    public LayerMask playerLayer;
    public LayerMask obstacleMask;
    public float timeToLosePlayer = 3.0f;

    [Header("Attack")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    [Header("Animation (optional)")]
    public Animator animator; // optional parameter animator

    NavMeshAgent agent;
    int currentWaypoint = 0;
    float waitTimer = 0f;
    bool playerInSight = false;
    float loseSightTimer = 0f;
    float attackTimer = 0f;

    enum State { Patrol, Chasing, Searching }
    State state = State.Patrol;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

        if (waypoints.Length == 0)
        {
            Debug.LogWarning("❗ MonsterAI: Belum ada waypoint.");
            agent.isStopped = true;
            return;
        }

        agent.speed = patrolSpeed;
        GotoNextWaypoint();
        if (animator) animator.SetBool("isCrawling", true);
    }

    void Update()
    {
        if (player == null)
        {
            GameObject pgo = GameObject.FindGameObjectWithTag("Player");
            if (pgo) player = pgo.transform;
        }

        playerInSight = CheckPlayerInSight();

        switch (state)
        {
            case State.Patrol:
                PatrolUpdate();
                if (playerInSight) StartChasing();
                break;

            case State.Chasing:
                ChaseUpdate();
                break;

            case State.Searching:
                SearchUpdate();
                break;
        }

        attackTimer -= Time.deltaTime;
    }

    #region Patrol
    void PatrolUpdate()
    {
        if (agent.pathPending) return;

        if (agent.remainingDistance <= waypointStopDistance)
        {
            if (waitTimer <= 0f)
            {
                waitTimer = waitAtWaypoint;
                agent.isStopped = true;
            }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    GotoNextWaypoint();
                    agent.isStopped = false;
                }
            }
        }
    }

    void GotoNextWaypoint()
    {
        if (waypoints.Length == 0) return;
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        agent.speed = patrolSpeed;
        agent.SetDestination(waypoints[currentWaypoint].position);
    }
    #endregion

    #region Chasing
    void StartChasing()
    {
        state = State.Chasing;
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        if (animator) animator.SetBool("isChasing", true);
        loseSightTimer = timeToLosePlayer;
    }

    void ChaseUpdate()
    {
        if (player == null) return;

        agent.SetDestination(player.position);
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            agent.isStopped = true;
            if (attackTimer <= 0f)
            {
                DoAttack();
                attackTimer = attackCooldown;
            }
        }
        else
        {
            agent.isStopped = false;
        }

        if (!playerInSight)
        {
            loseSightTimer -= Time.deltaTime;
            if (loseSightTimer <= 0f)
            {
                state = State.Searching;
                if (animator) animator.SetBool("isChasing", false);
                agent.SetDestination(player.position);
                agent.speed = patrolSpeed;
            }
        }
        else
        {
            loseSightTimer = timeToLosePlayer;
        }
    }

    void SearchUpdate()
    {
        if (!agent.pathPending && agent.remainingDistance <= waypointStopDistance)
        {
            state = State.Patrol;
            GotoNextWaypoint();
            if (animator) animator.SetBool("isChasing", false);
        }

        if (playerInSight)
        {
            StartChasing();
        }
    }
    #endregion

    void DoAttack()
    {
        Debug.Log("💀 SCP menangkap player! Game Over!");
        if (animator) animator.SetTrigger("isAttacking");

        // 💥 Langsung kill player:
        // 1️⃣ Disable movement player (kalau ada script movement)
        MonoBehaviour movement = player.GetComponent<MonoBehaviour>();
        if (movement != null) movement.enabled = false;

        // 2️⃣ Tampilkan efek atau animasi
        // (kamu bisa panggil GameOver UI di sini kalau punya)

        // 3️⃣ Restart Scene setelah delay
        StartCoroutine(RestartScene());
    }

    System.Collections.IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(2f); // beri waktu animasi SCP menyerang
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #region Detection
    bool CheckPlayerInSight()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = player.position - transform.position;
        float distToPlayer = dirToPlayer.magnitude;
        if (distToPlayer > viewRadius) return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer.normalized);
        if (angle > viewAngle * 0.5f) return false;

        // Raycast untuk line of sight
        if (Physics.Raycast(transform.position + Vector3.up * 0.6f, dirToPlayer.normalized, out RaycastHit hit, viewRadius))
        {
            if (hit.transform == player)
                return true;
        }

        return false;
    }
    #endregion

    #region Gizmos
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftDir = DirFromAngle(-viewAngle / 2, false);
        Vector3 rightDir = DirFromAngle(viewAngle / 2, false);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftDir * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * viewRadius);
    }

    Vector3 DirFromAngle(float angleDeg, bool global)
    {
        if (!global) angleDeg += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleDeg * Mathf.Deg2Rad), 0, Mathf.Cos(angleDeg * Mathf.Deg2Rad));
    }
    #endregion
}
