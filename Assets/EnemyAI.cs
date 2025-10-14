using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float chaseRange = 15f;
    public float attackRange = 2f;
    public float moveSpeed = 3.5f;

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= chaseRange && distance > attackRange)
        {
            ChasePlayer();
        }
        else if (distance <= attackRange)
        {
            AttackPlayer();
        }
        else
        {
            StopChasing();
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.speed = moveSpeed;
        agent.SetDestination(player.position);

        if (animator != null)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
        }
    }

    void AttackPlayer()
    {
        agent.isStopped = true;
        transform.LookAt(player);

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
        }
    }

    void StopChasing()
    {
        agent.isStopped = true;

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
