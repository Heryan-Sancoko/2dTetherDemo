using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostZone : MonoBehaviour
{

    [SerializeField] private float boostSpeed;
    [SerializeField] private float boostDuration;
    private PlayerController player;
    private bool finishedBoosting = true;

    public void BoostPlayer()
    {
        player.transform.position = transform.position;
        player.CancelTether();
        player.ForceVelocityOverDuration(transform.forward * boostSpeed, boostDuration, false, MoveStatus.boosting);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Constants.Layers.Player)
        {
            if (player == null)
                player = EnemyManager.Instance.PlayerController;

            if (finishedBoosting == false)
                return;

            Debug.LogError("BOOSTING");
            finishedBoosting = false;
            player.SetJumpAmount(1);
            BoostPlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Constants.Layers.Player)
        {
            finishedBoosting = true;
        }
    }
}
