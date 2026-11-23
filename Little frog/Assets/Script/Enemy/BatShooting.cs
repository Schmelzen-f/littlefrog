
using UnityEngine;
using System.Collections.Generic;

public class BatShooting : EnemyAttackBehaviour
{
    [Header("Shooting")]
    public float shootingRange = 3f;
    public float shootCooldown = 1.5f;
    private List<GameObject> activeBullets = new List<GameObject>();

    [Header("Projectile")]
    public GameObject bulletPrefab;
    public Transform shootPoint;

    private float cooldownTimer;

    public override float AttackRange => shootingRange;

    public override bool IsAttacking => false;

    public override bool CanStartAttack => cooldownTimer <= 0f;

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public override void StartAttack(Transform target)
    {
        if (!CanStartAttack || target == null)
            return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            shootPoint.position,
            Quaternion.identity
        );




        bullet.GetComponent<Shooting>().ShootAt(target, GetComponent<EnemyAttack>());




        activeBullets.Add(bullet);

        if (activeBullets.Count > 3)
        {
            Destroy(activeBullets[0]);
            activeBullets.RemoveAt(0);
        }



        cooldownTimer = shootCooldown;
    }
}
