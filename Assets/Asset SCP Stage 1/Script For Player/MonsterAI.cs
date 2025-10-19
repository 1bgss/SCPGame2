using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float waypointStopDistance = 0.3f;
    public float waitAtWaypoint = 1f;
    public float patrolSpeed = 2f;

    [Header("Chase Settings")]
    public float chaseSpeed = 3.5f;
    public float speedBoostMultiplier = 2f;

    [Header("Detection Settings")]
    public Transform player;
    public float viewRadius = 8f;
    [Range(0, 360)]
    public float viewAngle = 60f;
    public LayerMask obstacleMask;
    public float timeToLosePlayer = 3f;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;

    [Header("Scene Transition")]
    public string youDiedScene = "YouDied";

    [Header("Animation (Optional)")]
    public Animator animator;

    [Header("Sound Settings")]
    public AudioSource audioSource; // satu-satunya AudioSource
    public AudioClip footstepClip;  // loop langkah
    public AudioClip growlClip;     // growl/teriak
    public float growlInterval = 10f; // setiap 10 detik
    public float footstepVolume = 0.6f;
    public float footstepPitch = 1.0f;
    public float footstepChaseVolume = 1.0f;
    public float footstepChasePitch = 1.3f;

    private float growlTimer = 0f;

    private NavMeshAgent agent;
    private int currentWaypoint = 0;
    private float waitTimer = 0f;
    private bool playerInSight = false;
    private float playerLostTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            agent = gameObject.AddComponent<NavMeshAgent>();

        if (waypoints.Length > 0)
            agent.SetDestination(waypoints[0].position);

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;

        if (footstepClip != null)
        {
            audioSource.clip = footstepClip;
            audioSource.volume = footstepVolume;
            audioSource.pitch = footstepPitch;
        }
    }

    void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
            return;
        }

        // 🔊 Suara growl
        HandleGrowlSound();

        // 🔊 Suara langkah
        HandleFootstepSound();

        // 👀 Deteksi player
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

        // 🧠 Logika AI utama
        if (playerInSight)
            ChasePlayer();
        else
            Patrol();
    }

    void HandleGrowlSound()
    {
        growlTimer += Time.deltaTime;
        if (growlTimer >= growlInterval)
        {
            if (growlClip != null)
            {
                audioSource.PlayOneShot(growlClip, 1f); // main growl di atas clip langkah
            }
            growlTimer = 0f;
        }
    }

    void HandleFootstepSound()
    {
        if (footstepClip == null) return;

        bool isMoving = agent.velocity.magnitude > 0.1f && agent.remainingDistance > 0.2f;

        if (isMoving)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();

            // Sesuaikan volume & pitch saat chase/patrol
            if (playerInSight)
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, footstepChaseVolume, Time.deltaTime * 3f);
                audioSource.pitch = Mathf.Lerp(audioSource.pitch, footstepChasePitch, Time.deltaTime * 3f);
            }
            else
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, footstepVolume, Time.deltaTime * 3f);
                audioSource.pitch = Mathf.Lerp(audioSource.pitch, footstepPitch, Time.deltaTime * 3f);
            }
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        if (Vector3.Distance(transform.position, player.position) <= viewRadius)
        {
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position + Vector3.up * 0.6f, dirToPlayer, out RaycastHit hit, viewRadius, obstacleMask))
                    return true;
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

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float speedMultiplier = 1f + (speedBoostMultiplier - 1f) * (1f - Mathf.Clamp01(distanceToPlayer / viewRadius));
        agent.speed = chaseSpeed * speedMultiplier;
        agent.SetDestination(player.position);

        if (animator) animator.SetBool("isChasing", true);

        if (distanceToPlayer <= attackRange)
            KillPlayerInstant();
    }

    void KillPlayerInstant()
    {
        Debug.Log("💀 SCP langsung membunuh player! (Instant)");

        PlayerDeathEffect death = player.GetComponent<PlayerDeathEffect>();
        if (death != null)
            death.Die();

        if (agent) agent.isStopped = true;
        enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 a = DirFromAngle(-viewAngle / 2, false);
        Vector3 b = DirFromAngle(viewAngle / 2, false);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + a * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + b * viewRadius);
    }

    Vector3 DirFromAngle(float angleDeg, bool global)
    {
        if (!global) angleDeg += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleDeg * Mathf.Deg2Rad), 0, Mathf.Cos(angleDeg * Mathf.Deg2Rad));
    }
}
