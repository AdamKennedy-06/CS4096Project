using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class SwatAI : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sightRange = 12f;
    [SerializeField] private float shootRange = 7f;
    [SerializeField] private float reactionTime = 0.6f;
    [SerializeField] private float shotCooldown = 1.5f;
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private float damage = 5f;

    [SerializeField] private float turnSpeed = 8f;
    [SerializeField] private LayerMask sightBlockers = ~0;

    private UnityEngine.AI.NavMeshAgent agent;
    private Transform player;
    private float spottedTime = -1f;
    private float nextShotTime;
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    private bool wasChasing = false;
    private bool patrolDestinationSet = false;

    private void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.updateRotation = false;
    }
    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        
    }
    private void Update()
    {
       if(player == null)
        {
            if (wasChasing)
            {
                agent.ResetPath();
                wasChasing = false;
                patrolDestinationSet = false;
            }

            Patrol();
            return;
        }
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > sightRange || !HasLineOfSight())
        {
            spottedTime = -1f;

            if (wasChasing)
            {
                agent.ResetPath();
                wasChasing = false; 
            }
            Patrol();
            return;
        }

        wasChasing = true;
        agent.updateRotation = false;

        if (spottedTime < 0f)
            spottedTime = Time.time;
        FacePlayer();

        if(Time.time - spottedTime < reactionTime)
        {
            agent.isStopped = true;
            return;
        }

        if(distance > shootRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position); 
        }
        else
        {
            agent.isStopped = true; 
            TryShoot();
        }
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        agent.updateRotation = true;
        agent.isStopped = false;

        if (!patrolDestinationSet)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            patrolDestinationSet = true;
            return;
        }


        if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.25f)
        {
            currentPatrolIndex++;
            if (currentPatrolIndex >= patrolPoints.Length)
                currentPatrolIndex = 0;

            agent.SetDestination(patrolPoints[currentPatrolIndex].position); 
        }
    }
    private bool HasLineOfSight()
    {
        Vector3 eye = transform.position + Vector3.up * 0.5f;
        Vector3 target = player.position + Vector3.up * 0.5f;
        Vector3 dir = target - eye;
        if(Physics.Raycast(eye, dir.normalized, out RaycastHit hit, dir.magnitude, sightBlockers, QueryTriggerInteraction.Ignore))
        {
            return hit.transform == player || hit.transform.IsChildOf(player);
        }
        return true;
    }
    private void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.magnitude < 0.01f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
    }
    private void TryShoot()
    {
        if (Time.time < nextShotTime || bulletPrefab == null || firePoint == null) return;
        nextShotTime = Time.time + shotCooldown;
        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.Launch(bulletSpeed, damage);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }

}
