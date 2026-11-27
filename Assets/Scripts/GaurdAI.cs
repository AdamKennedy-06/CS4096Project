using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class GaurdAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    private EnemyNavMesh moveScript;
    private int whichPatrolPoint = 0;
    public float waitDuration = 3f;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    public float viewDistance = 10f;
    public float halfViewAngle = 45f;
    private GameObject player;
    private Transform playerPos;
    private bool visable = false;
    public float routeInterval = 0.3f;
    private float countdown;
    public float searchTime = 4f;
    private float lostTimer;
    public float atkRange = 1.9f;
    public float atkCooldown = 1.2f;
    private float atkTimer;
    public float atkDamage = 10f;
    private IDamageable PlayerHealth;
    private ThirdPersonController playerController;
    [SerializeField] private float stunDuration = 0f;
    public Bullet taser;
    public Transform firePoint;
    public float boltspeed;
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    EnemyState currentState = EnemyState.Patrol;
    void Awake()
    {
        moveScript = GetComponent<EnemyNavMesh>();
    }
    // Start is called before the first frame update
    void Start()
    {
      

        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.Log("Player Game Object not found with tag Player");
            return;
        }
        playerPos = player.transform;
        PlayerHealth = player.GetComponent<PlayerHealth>();
        playerController = player.GetComponent<ThirdPersonController>();

        if (patrolPoints.Length == 0)
        {
            return;
        }

        moveScript.RouteCalculator(patrolPoints[whichPatrolPoint].position);

    }

    void StartWaiting()
    {
        isWaiting = true;
        waitTimer = waitDuration;
    }

    // Update is called once per frame
    void Update()
    {
        bool newAnswer = HasLineOfSight();

        if (newAnswer != visable)
        {
            if (newAnswer)
            {
                Debug.Log("Player spotted");
            }
            else
            {
                Debug.Log("Player lost");
            }
        }

        visable = newAnswer;

        //Drawing line for Sight Testing
        Color coneColour;
        if (visable == true)
        {
            coneColour = Color.green;
        }
        else
        {
            coneColour = Color.red;
        }


        Vector3 startPoint = transform.position + Vector3.up * 1.4f;
        Vector3 lengDist = transform.forward * viewDistance;
        Debug.DrawRay(startPoint, lengDist, coneColour);

        Quaternion rightAngleView = Quaternion.AngleAxis(halfViewAngle, Vector3.up);
        Vector3 rightAngleLine = rightAngleView * transform.forward;
        Debug.DrawRay(startPoint, rightAngleLine * viewDistance, coneColour);

        Quaternion leftAngleView = Quaternion.AngleAxis(-halfViewAngle, Vector3.up);
        Vector3 leftAngleLine = leftAngleView * transform.forward;
        Debug.DrawRay(startPoint, leftAngleLine * viewDistance, coneColour);

        switch (currentState)
        {
            case EnemyState.Patrol:
                PatrolUpdate();
                break;

            case EnemyState.Chase:
                ChaseUpdate();
                break;

                case EnemyState.Attack:
                AttackUpdate();
                    break;
        }













    }

    public bool HasLineOfSight()
    {
        Vector3 playerTarget = playerPos.position;
        Vector3 direction = playerTarget - transform.position;
        direction.y = 0;

        if (Vector3.Distance(transform.position, playerTarget) > viewDistance)
        {
            return false;
        }


        if (Vector3.Angle(transform.forward, direction) > halfViewAngle)
        {

            return false;
        }

        Vector3 aiEyeHeight = transform.position + Vector3.up * 1.4f;
        Vector3 playerBodyPosition = playerTarget + Vector3.up * 1.0f;
        Vector3 distance = playerBodyPosition - aiEyeHeight;
        float rayDistance = Vector3.Distance(aiEyeHeight, playerBodyPosition);

        RaycastHit hit;
        if(Physics.Raycast(aiEyeHeight,distance.normalized, out hit, rayDistance))
        {
            if (!hit.transform.IsChildOf(playerPos)){
                return false;
            }
        }

        return true;

        
    }

    public void PatrolUpdate()
    {
        if (HasLineOfSight() == true)
        {
            StateChange(EnemyState.Chase);
            return;
        }


        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                whichPatrolPoint = (whichPatrolPoint + 1) % patrolPoints.Length;
                moveScript.RouteCalculator(patrolPoints[whichPatrolPoint].position);
            }
            return;
        }

        if (patrolPoints.Length == 0)
        {
            return;
        }

        if (moveScript.HasFinishedPath() == true)
        {
            StartWaiting();
        }
    }

    public void StateChange(EnemyState newState)
    {
        Debug.Log("State Changed " + newState);
        currentState = newState;
        if (newState == EnemyState.Chase)
        {
            countdown = 0;
            lostTimer = searchTime;
        }
        if (newState == EnemyState.Patrol)
        {
            if (patrolPoints.Length > 0)
            {
                moveScript.RouteCalculator(patrolPoints[whichPatrolPoint].position);
            }
        }
        if (newState == EnemyState.Attack)
        {
            atkTimer = 0f;
            moveScript.StopMovement();

        }

        isWaiting = false;


    }

    public void ChaseUpdate()
    {
        if (visable == true)
        {
            lostTimer = searchTime;
        }
        else
        {
            lostTimer = lostTimer - Time.deltaTime;
        }

        if (lostTimer <= 0)
        {
            StateChange(EnemyState.Patrol);
            return;
        }
        countdown = countdown - Time.deltaTime;
        if (countdown <= 0f)
        {
            moveScript.RouteCalculator(playerPos.position);
            countdown = routeInterval;
        }

        
        float dist = Vector3.Distance(playerPos.position, transform.position);
        if (visable == true && dist <= atkRange)
        {
            StateChange(EnemyState.Attack);
        }


    }

    public void AttackUpdate()
    {
        float dist = Vector3.Distance(playerPos.position, transform.position);
        if (dist > atkRange || visable == false)
        {
            StateChange(EnemyState.Chase);
            return;
        }
        moveScript.RotationCheck(playerPos.position);
        atkTimer = atkTimer - Time.deltaTime;
        if (atkTimer <= 0f)
        {
            atkTimer = atkCooldown;
            if (PlayerHealth != null)
            {
                PlayerHealth.TakeDamage(atkDamage);
                if(stunDuration > 0f && playerController != null)
                {
                    if (taser != null && firePoint != null)
                    {
                        Bullet bolt = Instantiate(taser, firePoint.position, firePoint.rotation);
                        bolt.Launch(boltspeed, 0f);

                    }
                    playerController.ApplyStun(stunDuration);
                    Debug.Log("Police Stun!!");
                }
            }
            Debug.Log("Cop Attacks!!");
        }
       
    }


}
