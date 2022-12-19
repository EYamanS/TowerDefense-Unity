using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    protected virtual float secondPerBullet { get; set; } = .2f;
    protected virtual float damagePerShot { get; set; } = 10f;

    public List<Enemy> enemiesInRange = new List<Enemy>();

    [SerializeField] private LayerMask _enemyLayer;
    private List<Enemy> theyAreAlreadyDead = new List<Enemy>();

    protected float timer = 0f;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy hit))
        {
            enemiesInRange.Add(hit);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy hit))
        {
            enemiesInRange.Remove(hit);
        }
    }

    private void Start()
    { 
    
        damagePerShot = Random.Range(2f, 10f);
    }

    protected virtual void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0.0f && enemiesInRange.Count > 0)
        {
            foreach (Enemy enemyInRange in enemiesInRange)
            {
                if (enemyInRange.myHealthWillBe > 0.0f)
                {
                    enemyInRange.myHealthWillBe -= damagePerShot;
                    Shoot(enemyInRange);
                    timer = secondPerBullet;
                    break;
                }
            }

        }
    }


    public virtual void Shoot(Enemy target)
    {


        Bullet blt = BulletPool.Instance.SpawnBullet().GetComponent<Bullet>();
        blt.gameObject.transform.position = transform.position;
        blt.target = target.transform;
        blt.speed = target.speed + 5f;
        blt.damage = damagePerShot;
    }





}
