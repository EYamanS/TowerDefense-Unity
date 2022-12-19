using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocketeer : Turret
{
    new protected float damagePerShot = 50f;
    new protected float secondPerBullet = 3f;



    protected override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0.0f && enemiesInRange.Count > 0)
        {
            foreach (Enemy enemyInRange in enemiesInRange)
            {
                if (enemyInRange.myHealthWillBe > 0.0f)
                {
                    Shoot(enemyInRange);
                    timer = secondPerBullet;
                    break;
                }
            }

        }
    }
    public override void Shoot(Enemy target)
    {
        target.myHealthWillBe -= damagePerShot;

        Rocket blt = BulletPool.Instance.SpawnRocket().GetComponent<Rocket>();
        blt.gameObject.transform.position = transform.position;
        blt.target = target.transform;
        blt.speed = target.speed + 2f;
        blt.damage = damagePerShot;
    }
}
