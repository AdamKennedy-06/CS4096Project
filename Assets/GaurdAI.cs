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

    void Awake()
    {
        moveScript = GetComponent<EnemyNavMesh>();
    }
    // Start is called before the first frame update
    void Start()
    {
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

