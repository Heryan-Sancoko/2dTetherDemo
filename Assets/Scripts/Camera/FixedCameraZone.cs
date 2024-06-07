using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCameraZone : MonoBehaviour
{
    [SerializeField] private float cameraZoomAmount;
    [SerializeField] private Vector3 cameraPosition;
    [SerializeField] private Camera mainCam;
    [SerializeField] CameraController camControl;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AdjustCameraForArea()
    {
        camControl.FixCamera(cameraZoomAmount, cameraPosition);
    }

    private void ResetToFollowCamera()
    {
        camControl.SetCamStyle(CameraStyle.FollowLookDirection);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Constants.Layers.Player)
        {
            AdjustCameraForArea();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Constants.Layers.Player)
        {
            ResetToFollowCamera();
        }
    }
}
