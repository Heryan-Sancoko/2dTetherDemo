using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

//THIS SCRIPT IS STILL A WIP. DISABLE IT TO RUN NORMALLY.

public class TetherModule : PlayerModule
{

    [SerializeField] private Transform tetherAnchor;
    [SerializeField] private Transform swingAssistant;
    [SerializeField] private GameObject nodePrefab;

    private PlayerMovementModule playerMovementModule;
    private bool isUsingMouse = false;
    private Camera mcam;

    [SerializeField] private Vector3 tetherDirection;
    [SerializeField] private bool swingClockwise;
    [SerializeField] private Vector3 swingdDirection;
    [SerializeField] private List<GameObject> tetherNodes = new List<GameObject>();
    [SerializeField] private List<GameObject> inactiveNodes = new List<GameObject>();
    [SerializeField] private float tetherSpeed;
    [SerializeField] private float nodeDistanceFromCollision;
    [SerializeField] private float swingSpeed;
    [SerializeField] private float swingAcceleration;
    [SerializeField] private float tetherRange;
    [SerializeField] private float tetherAnchorRadius;
    [SerializeField] private float ropeTension;
    [SerializeField] private LayerMask tetherMask;
    [SerializeField] private LineRenderer lineRenderer;

    public enum TetherState {idle, shooting, anchored, swinging}
    private TetherState currentTetherState;
    private float ClosestDistance;
    private float currentSwingSpeed;


    private void Start()
    {
        InputManager.Instance.Tether.performed += OnTether;
        InputManager.Instance.Tether.canceled += OnTetherCancelled;

        mcam = Camera.main;
        playerMovementModule = playerController.GetModule<PlayerMovementModule>();
        isUsingMouse = true;
        //isUsingMouse = Gamepad.current == null;
    }

    public override void UpdatePlayerModule()
    {
        base.UpdatePlayerModule();

    }

    public override void FixedUpdatePlayerModule()
    {
        base.FixedUpdatePlayerModule();

        switch (currentTetherState)
        {
            case TetherState.anchored:
                MovePlayerToAnchor();
                CheckForSwingDirection();
                RenderTetherRope();
                break;
            case TetherState.swinging:
                ApplySwingDirectionVelocity();
                CheckTetherCollisionWhileSwinging();
                RenderTetherRope();
                break;
        }
    }

    private void OnTether(InputAction.CallbackContext callback)
    {
        if (InputManager.Instance.Tether.WasPressedThisFrame())
        {
            //Debug.LogError("TETHER DOWN");
            playerController.SetCurrentMoveStatus(MoveStatus.tethering);
            PlantTetherAnchor();
        }
    }

    //cast ray out
    //get hit point
    //anchor is placed on hit position
    //anchor becomes a child of the terrain
    private void PlantTetherAnchor()
    {
        if (isUsingMouse)
        {
            Vector3 mousePos = mcam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            tetherDirection = (mousePos - transform.position).normalized;
        }

        //for now it's instant, but later, we might want to make the player actually throw the tether forward
        //but that could cause some bugs like the rope clipping into geometry as the player is still moving around
        //so for now, we'll just keep it simple
        if (Physics.SphereCast(transform.position, tetherAnchorRadius, tetherDirection, out RaycastHit hitinfo, tetherRange))
        {
            tetherAnchor.position = hitinfo.point;
            tetherAnchor.SetParent(hitinfo.transform, true);
            tetherAnchor.localScale = new Vector3(1 / hitinfo.transform.localScale.x, 1 / hitinfo.transform.localScale.y, 1 / hitinfo.transform.localScale.z);
            currentTetherState = TetherState.anchored;
            ClosestDistance = Vector3.Distance(transform.position, hitinfo.point);
            SetUnusedNodePosition(GetUnusuedNode(), tetherAnchor.position, hitinfo.transform);
        }

    }

    //player moves towards anchor
    //if player reaches anchor, player remains in place until tether is released
    //if player inputs a direction...
    private void MovePlayerToAnchor()
    {
        Vector3 tetherTowards = GetCurrentTetherPoint();

        Vector3 tetherVelocity = (tetherTowards - transform.position).normalized * tetherSpeed;
        playerController.ApplyNewVelocityToRigidbody(tetherVelocity);
    }

    private Vector3 GetCurrentTetherPoint()
    {
        Vector3 tetherTowards = tetherAnchor.position;

        if (tetherNodes.Count == 0)
        {
            SetUnusedNodePosition(GetUnusuedNode(), tetherAnchor.position, tetherAnchor);
        }

        tetherTowards = tetherNodes[tetherNodes.Count - 1].transform.position;
        return tetherTowards;

    }

    //find the direction of input, compare to a dot product vs tether direction
    //begin swinging player in direction by...
    //swingAssist looks at anchor, apply positive or negative x speed
    //confine the player to a certain distance by taking the speed of which we are moving away from the anchor and applying an inverse force
    private void MoveSwingAssistant()
    {
        swingAssistant.transform.position = transform.position;
        swingAssistant.transform.LookAt(GetCurrentTetherPoint(), -transform.forward);
    }

    private void CheckForSwingDirection()
    {
        MoveSwingAssistant();
        if (playerMovementModule.InputVector != Vector2.zero)
        {
            float MovementToTetherDot = Vector3.Dot(swingAssistant.right, playerMovementModule.InputVector);

            swingClockwise = (MovementToTetherDot > 0);
            currentSwingSpeed = tetherSpeed;
            currentTetherState = TetherState.swinging;
        }
    }

    private void ApplySwingDirectionVelocity()
    {
        Vector3 currentTether = GetCurrentTetherPoint();

        CheckRopeTension(currentTether);

        MoveSwingAssistant();

        Vector3 SwingVec = (swingClockwise) ?
            swingAssistant.right :
            -swingAssistant.right;

        Vector3 localSwing = swingAssistant.InverseTransformDirection(SwingVec * currentSwingSpeed);
        currentSwingSpeed = Mathf.Lerp(currentSwingSpeed, swingSpeed, swingAcceleration);
        localSwing.z = Mathf.Clamp((Vector3.Distance(transform.position, currentTether) - ClosestDistance) *ropeTension, 0, 999);
        swingdDirection = swingAssistant.TransformDirection(localSwing);

        playerMovementModule.ApplyNewVelocityToRigidbody(swingdDirection);
    }

    private void CheckRopeTension(Vector3 currentTether)
    {
        if (Vector3.Distance(transform.position, currentTether) < ClosestDistance)
            ClosestDistance = Vector3.Distance(transform.position, GetCurrentTetherPoint());
    }

    //cast a sphere to the anchor while we're swinging
    //if the spherecast collides with anything, create a node at the point, make the node a child of the terrain
    //using the swingAssist local x, move the node a certain distance away from the hitpoint
    //we now swing from the latest tether node
    private void CheckTetherCollisionWhileSwinging()
    {
        Vector3 currentTetherPoint = GetCurrentTetherPoint();

        if (Physics.Raycast(transform.position, currentTetherPoint - transform.position, out RaycastHit hitinfo, Vector3.Distance(transform.position, currentTetherPoint) * 0.99f, tetherMask))
        {
            Vector3 newNodePosition = hitinfo.point;
            newNodePosition -= swingdDirection.normalized*nodeDistanceFromCollision;
            SetUnusedNodePosition(GetUnusuedNode(), newNodePosition, hitinfo.transform);
        }
        else
        {
            RenderTetherRope();
        }
    }

    private void SetUnusedNodePosition(GameObject newNode, Vector3 newNodePosition, Transform nodeParent)
    {
        if (newNode != null)
        {
            tetherNodes.Add(newNode);
            newNode.transform.position = newNodePosition;
            newNode.transform.parent = nodeParent;
        }
        else
        {
            newNode = Instantiate(nodePrefab, nodeParent);
            newNode.transform.position = newNodePosition;
            tetherNodes.Add(newNode);
        }
        RedrawLineRendererWithNewNode(newNode);
    }

    private GameObject GetUnusuedNode()
    {
        if (inactiveNodes.Count != 0)
        {
            GameObject inactiveNode = inactiveNodes[0];
            inactiveNodes.Remove(inactiveNodes[0]);
            return inactiveNode;
        }

        return null;
    }

    //tether nodes should be poolable
    //tether nodes should be a prefab

    private void RedrawLineRendererWithNewNode(GameObject newNode)
    {
        Vector3[] nodePositionArray = new Vector3[tetherNodes.Count + 1];
        for (int i = 0; i < nodePositionArray.Length; i++)
        {
            if (i == nodePositionArray.Length - 1)
            {
                nodePositionArray[i] = transform.position;
            }
            else
            {
                nodePositionArray[i] = tetherNodes[i].transform.position;
            }
        }

        lineRenderer.positionCount = nodePositionArray.Length;
        lineRenderer.SetPositions(nodePositionArray);
    }


    private void RenderTetherRope()
    {
        RedrawLineRendererWithNewNode(null);
        //lineRenderer.SetPosition(lineRenderer.positionCount-1, transform.position);
    }

    private void OnTetherCancelled(InputAction.CallbackContext callback)
    {
        //Debug.LogError("TETHER UP");
        currentTetherState = TetherState.idle;
        playerController.SetCurrentMoveStatus(MoveStatus.passive);
        playerMovementModule.SetJumpAmount(1);
        inactiveNodes.AddRange(tetherNodes);
        tetherNodes.Clear();
        
    }



}
