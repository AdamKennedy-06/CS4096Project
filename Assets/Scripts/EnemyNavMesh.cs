using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyNavMesh : MonoBehaviour
{
    public int currentCornerIndex = 0;
    public float MovementSpeed = 2.5f;
    private Vector3 destination;
    private NavMeshPath myCustomPath;
    [SerializeField] private Transform movePosition;


    // Start is called before the first frame update
    void Start()
    {
        RouteCalculator(movePosition.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(myCustomPath == null || myCustomPath.corners.Length == 0)
        {
            return;
        }

        if(currentCornerIndex >= myCustomPath.corners.Length)
        {
           
            return;
        }

        Vector3 patrolTarget = myCustomPath.corners[currentCornerIndex];

        float maxDistanceDelta = MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, patrolTarget, maxDistanceDelta);

        if ((Vector3.Distance(transform.position, patrolTarget) < 0.2)){
            currentCornerIndex++;
        } 
        


        for (int i = 0;  i < myCustomPath.corners.Length - 1; i++){
            Debug.DrawLine(myCustomPath.corners[i], myCustomPath.corners[i+1], Color.red, 2.5f);
        }
    }

    public void RouteCalculator(Vector3 targetPosition)
    {
        myCustomPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, myCustomPath);
        Debug.Log("Number of corners = " + myCustomPath.corners.Length);
        switch (myCustomPath.status)
        {
            case NavMeshPathStatus.PathComplete:
                currentCornerIndex = 0;
                break;

            case NavMeshPathStatus.PathPartial:
                Debug.Log("Path is Partially Complete.");
                break;

            case NavMeshPathStatus.PathInvalid:
                Debug.Log("Path is Incomplete.");
                break;
        }

    }
}

