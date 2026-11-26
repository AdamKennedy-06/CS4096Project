using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    enum EnemyState
    {
        Patrol,
        Chase
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
        if(player == null){
            Debug.Log("Player Game Object not found with tag Player");
            return;
        }
        playerPos = player.transform;

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
        if(visable == true)
        {
            coneColour = Color.green;
        } else {
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
               
                break;

            /*case EnemyState.Attack:
                
                break; */
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

        return true;

    }

    public void PatrolUpdate()
    {
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

   
}

