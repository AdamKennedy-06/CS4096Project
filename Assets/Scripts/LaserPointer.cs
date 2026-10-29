using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class LaserPointer : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private LayerMask hitMask = ~0;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
    }

    private void Update()
    {
        Vector3 start = firePoint.position;
        Vector3 end = start + firePoint.forward * maxDistance;

        if(Physics.Raycast(start, firePoint.forward, out RaycastHit hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore))
        {
            end = hit.point;
        }
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }
}
