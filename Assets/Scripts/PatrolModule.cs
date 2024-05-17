using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolModule : MonoBehaviour
{

    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float patrolSpeed;
    private bool isTargetPointB = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isTargetPointB)
        {
            if (Vector3.Distance(transform.position, pointB.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, pointB.position, patrolSpeed);
            }
            else
            {
                isTargetPointB = false;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, pointA.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, pointA.position, patrolSpeed);
            }
            else
            {
                isTargetPointB = true;
            }
        }
    }
}
