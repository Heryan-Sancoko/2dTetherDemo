using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType { standardBullet}


public class BulletScript : MonoBehaviour
{

    [SerializeField] private float bulletBaseDamage;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletRange;
    [SerializeField] private float bulletLifetime;
    private float bulletLifetimeRemaining;
    private Vector3 bulletOrigin;
    private Rigidbody rbody;
    private bool isPlayerBullet = false;
    private float bonusDamage;
    public BulletType bulletType;

    // Start is called before the first frame update
    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        if (!BulletManager.Instance.bulletList.Contains(this))
        {
            BulletManager.Instance.bulletList.Add(this);
        }
    }

    public void ConfigureBullet(Vector3 forwardVector, Vector3 newBulletOrigin, float newBonusDamage = 0, bool newIsPlayerBullet = false)
    {
        rbody.velocity = Vector3.zero;
        rbody.angularVelocity = Vector3.zero;
        transform.position = bulletOrigin = newBulletOrigin;

        Debug.LogError("Origin is: " + newBulletOrigin);
        transform.LookAt(transform.position + (forwardVector * 10), Vector3.up);
        bonusDamage = newBonusDamage;
        isPlayerBullet = newIsPlayerBullet;
        bulletLifetimeRemaining = bulletLifetime;
        gameObject.layer = isPlayerBullet ? Constants.Layers.PlayerProjectile : Constants.Layers.EnemyProjectile;
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Vector3.Distance(transform.position, bulletOrigin) > bulletRange)
        {
            gameObject.SetActive(false);
        }

        if (bulletLifetimeRemaining > 0)
        {
            bulletLifetimeRemaining -= Time.deltaTime;
            if (bulletLifetimeRemaining <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    void FixedUpdate()
    {
        rbody.velocity = transform.forward * bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isPlayerBullet)
        {
            if (collision.gameObject.layer == Constants.Layers.Player)
            {
                Debug.LogError("This is where you would take damage if I added player health");
            }
        }
        else
        {
            if (collision.gameObject.layer == Constants.Layers.Enemy)
            {
                if (collision.gameObject.TryGetComponent(out EnemyController enemyController))
                {
                    enemyController.TakeDamage(bulletBaseDamage + bonusDamage);
                }
            }
        }

        gameObject.SetActive(false);
    }
}
