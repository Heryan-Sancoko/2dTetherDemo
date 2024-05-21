using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundedCheckModule : PlayerModule
{
    [SerializeField] private bool isGrounded;
    public bool IsGrounded => isGrounded;

    [SerializeField] private float groundedSphereCheckRadius;
    [SerializeField] private float groundedDistanceCheck;
    [SerializeField] private LayerMask groundedLayerMask;

    public UnityAction JustLanded;

    private Vector3 hitPoint;
    public Vector3 HitPoint => hitPoint;

    private Rigidbody rbody;

    public override void AddController(EntityController newController)
    {
        base.AddController(newController);
        rbody = GetComponent<Rigidbody>();
    }

    public override void UpdatePlayerModule()
    {
        base.UpdatePlayerModule();

        Vector3 radiusOffset = (Vector3.up * (groundedSphereCheckRadius * 0.5f));

        if (Physics.SphereCast(transform.position, groundedSphereCheckRadius, Vector3.down, out RaycastHit hitinfo, groundedDistanceCheck, groundedLayerMask))
        {
            if (!isGrounded && rbody.velocity.y <= 0)
            {
                isGrounded = true;
                hitPoint = hitinfo.point + radiusOffset;
                JustLanded?.Invoke();
            }

        }
        else
        {
            isGrounded = false;
            hitPoint = (transform.position + Vector3.down * groundedDistanceCheck) + radiusOffset;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(hitPoint, groundedSphereCheckRadius);
    }
}
