using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCameraZone : MonoBehaviour
{
    [SerializeField] private float cameraZoomAmount;
    [SerializeField] private Vector3 cameraPosition;
    [SerializeField] private Camera mainCam;
    [SerializeField] CameraController camControl;

    [SerializeField] private List<EnemyController> enemiesToEnable = new List<EnemyController>();
    [SerializeField] private List<GameObject> doorsToLockUponEntering = new List<GameObject>();
    [SerializeField] private List<GameObject> doorsToOpenUponCompletion = new List<GameObject>();

    private bool enemiesSpawnedAlready = false;

    private int killedEnemiesInArea = 0;


    // Start is called before the first frame update
    void Start()
    {
        foreach (EnemyController enemy in enemiesToEnable)
        {
            if (enemy.TryGetComponent(out EnemyHealthModule enemyHealth))
            {
                enemyHealth.KillEntity += OnEnemiesKilled;
            }
        }
    }

    private void OnEnemiesKilled()
    {
        killedEnemiesInArea++;
        if (killedEnemiesInArea >= enemiesToEnable.Count)
        {
            OpenDoors();
        }
    }

    private void OpenDoors()
    {
        foreach (GameObject door in doorsToOpenUponCompletion)
        {
            door.gameObject.SetActive(false);
        }
    }

    private void CloseDoors()
    {
        foreach (GameObject door in doorsToLockUponEntering)
        {
            door.gameObject.SetActive(true);
        }
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

            if (!enemiesSpawnedAlready)
            {
                foreach (EnemyController enemy in enemiesToEnable)
                {
                    enemy.gameObject.SetActive(true);
                    enemy.ToggleForceEnemyIdle(false);
                }
                CloseDoors();
                enemiesSpawnedAlready = true;
            }
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
