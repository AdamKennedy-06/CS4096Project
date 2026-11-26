using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyNavMesh : MonoBehaviour
{
    public int currentCornerIndex = 0;
    public float MovementSpeed = 2.5f;
    private NavMeshPath myCustomPath;
    public float TurnSpeed = 180f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (HasFinishedPath())
        {
            return;
        }
        else
        {

            Vector3 patrolTarget = myCustomPath.corners[currentCornerIndex];

            //Movement
            float maxDistanceDelta = MovementSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, patrolTarget, maxDistanceDelta);

            if ((Vector3.Distance(transform.position, patrolTarget) < 0.2))
            {
                currentCornerIndex++;
            }
            //Rotate
            RotationCheck(patrolTarget);


            for (int i = 0; i < myCustomPath.corners.Length - 1; i++)
            {
                Debug.DrawLine(myCustomPath.corners[i], myCustomPath.corners[i + 1], Color.red, 2.5f);
            }
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
                currentCornerIndex = 0;
                break;

            case NavMeshPathStatus.PathInvalid:
                Debug.Log("Path is Incomplete.");
                currentCornerIndex = 0;
                break;
        }

    }

    public void RotationCheck(Vector3 targetposition)
    {
        Vector3 direction = targetposition - transform.position;
        direction.y = 0;

        if(direction.sqrMagnitude > 0.001)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, TurnSpeed * Time.deltaTime);

        }
       

    }

    public bool HasFinishedPath()
    {
        if (myCustomPath == null || myCustomPath.corners.Length == 0)
        {
            return true;
        }

        if (currentCornerIndex >= myCustomPath.corners.Length)
        {

            return true;
        }

        return false;
        
    }
}

