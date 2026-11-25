using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaurdAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    private EnemyNavMesh moveScript;
    private int whichPatrolPoint = 0;


    void Awake()
    {
       moveScript =  GetComponent<EnemyNavMesh>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(patrolPoints.Length == 0)
        {
            return;
        }

        moveScript(patrolPoints.Length);
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
