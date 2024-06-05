using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraStyle { EnemyAndPlayerAverage, FollowLookDirection, FixedCamera }

public class CameraController : MonoBehaviour
{ 

    [SerializeField] private List<Transform> focusObjectList = new List<Transform>();
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private float snapAmount;
    [SerializeField] private float cameraLeashLength;
    [SerializeField] private float mousePosLerpSpeed;
    [SerializeField] private float cameraLerpSpeed;
    [SerializeField] private CameraStyle CurrentCamStyle;
    [SerializeField] private float originalCameraSize;

    private float currentCameraSize;
    private Vector3 currentCameraPosition;

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
                if (mCam.orthographicSize != originalCameraSize)
                {
                    mCam.orthographicSize = Mathf.Lerp(mCam.orthographicSize, originalCameraSize, 0.1f);
                }
                FocusCameraOnPlayerAndAim(playerPos);
                break;

            case CameraStyle.FixedCamera:
                if (transform.position != currentCameraPosition)
                {
                    transform.position = Vector3.Lerp(transform.position, currentCameraPosition, cameraLerpSpeed);
                    mCam.orthographicSize = Mathf.Lerp(mCam.orthographicSize, currentCameraSize, cameraLerpSpeed);
                }
                break;
        }
    }

    public void SetCamStyle(CameraStyle newCameraStyle)
    {
        CurrentCamStyle = newCameraStyle;
    }

    private void FocusCameraOnPlayerAndAim(Vector3 playerPos)
    {
        Vector3 mousePos = mCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 newPos2 = Vector3.Lerp(mousePos, playerPos, snapAmount) - playerPos;
        transform.position = Vector3.Lerp(transform.position, playerPos + Vector3.ClampMagnitude(newPos2, cameraLeashLength), cameraLerpSpeed);
    }

    public void FixCamera(float newCameraSize, Vector3 newCameraPosition)
    {
        currentCameraSize = newCameraSize;
        currentCameraPosition = newCameraPosition;
        CurrentCamStyle = CameraStyle.FixedCamera;
    }

    private Vector3 GetAveragedCenter()
    {
        Vector3 averagePos = Vector3.zero;

        foreach (Transform objects in focusObjectList)
        {
            if (!objects.gameObject.activeSelf)
                continue;

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
