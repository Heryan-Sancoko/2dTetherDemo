using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private enum CameraStyle {EnemyAndPlayerAverage, FollowLookDirection}

    [SerializeField] private List<Transform> focusObjectList = new List<Transform>();
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private float snapAmount;
    [SerializeField] private float cameraLeashLength;
    [SerializeField] private float mousePosLerpSpeed;
    [SerializeField] private float cameraLerpSpeed;
    [SerializeField] private CameraStyle CurrentCamStyle;
    private Camera mCam;
    private float zOffset;

    private void Start()
    {
        mCam = GetComponent<Camera>();
        zOffset = transform.position.z;
    }

    private void FixedUpdate()
    {
        Vector3 playerPos = PlayerTransform.position;
        playerPos.z = zOffset;

        switch (CurrentCamStyle)
        {
            case CameraStyle.EnemyAndPlayerAverage:
                Vector3 averagePos = GetAveragedCenter();
                Vector3 newPos = Vector3.Lerp(transform.position, averagePos, snapAmount) - playerPos;
                Vector3 clamped = Vector3.ClampMagnitude(newPos, cameraLeashLength);
                transform.position = playerPos + clamped;
                break;

            case CameraStyle.FollowLookDirection:
                Vector3 mousePos = mCam.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                Vector3 newPos2 = Vector3.Lerp(mousePos, playerPos, snapAmount) - playerPos;
                transform.position = Vector3.Lerp(transform.position, playerPos + Vector3.ClampMagnitude(newPos2, cameraLeashLength), cameraLerpSpeed);
                break;
        }
    }

    private Vector3 GetAveragedCenter()
    {
        Vector3 averagePos = Vector3.zero;

        foreach (Transform objects in focusObjectList)
        {
            averagePos += objects.transform.position;
        }
        averagePos /= focusObjectList.Count;

        averagePos.z = zOffset;

        return averagePos;
    }

    public void AddTransformToFocusList(Transform newFocus)
    {
        if (!focusObjectList.Contains(newFocus))
        {
            focusObjectList.Add(newFocus);
        }
    }

    public void RemoveTransformFromFocusList(Transform cutFocus)
    {
        if (focusObjectList.Contains(cutFocus))
        {
            focusObjectList.Remove(cutFocus);
        }
    }
}
