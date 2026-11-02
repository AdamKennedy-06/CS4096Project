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


    // Start is called before the first frame update
    void Start()
    {

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

    }

    public void RouteCalculator(Vector3 targetPosition)
    {
        myCustomPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, myCustomPath);
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

