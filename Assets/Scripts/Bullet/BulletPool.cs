using System.Collections.Generic;
using UnityEngine;

public class BulletPool : Singleton<BulletPool>
{
    // The prefab for the bullets
    public GameObject bulletPrefab;
    public GameObject rocketPrefab;
    // The pool of available bullets
    private List<GameObject> bulletPool = new List<GameObject>();
    private List<GameObject> rocketPool = new List<GameObject>();


    // Spawn a bullet from the pool
    public GameObject SpawnBullet()
    {
        Bullet blt;
        // Find an inactive bullet in the pool
        GameObject bullet = bulletPool.Find(b => !b.activeInHierarchy && b.TryGetComponent<Bullet>(out blt));

        // If we found an inactive bullet, reset it and return it
        if (bullet != null)
        {
            bullet.SetActive(true);
            return bullet;
        }


        // If we didn't find an inactive bullet and the pool isn't full, instantiate a new bullet and add it to the pool
        bullet = Instantiate(bulletPrefab);
        bulletPool.Add(bullet);
        return bullet;
    }

    public GameObject SpawnRocket()
    {
        Rocket rkt;
        // Find an inactive bullet in the pool
        GameObject rocket = rocketPool.Find(b => !b.activeInHierarchy && b.TryGetComponent<Rocket>(out rkt));

        // If we found an inactive bullet, reset it and return it
        if (rocket != null)
        {
            rocket.SetActive(true);
            return rocket;
        }


        // If we didn't find an inactive bullet and the pool isn't full, instantiate a new bullet and add it to the pool
        rocket = Instantiate(rocketPrefab);
        rocketPool.Add(rocket);
        return rocket;
    }

    // Return a bullet to the pool
    public void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
    }

    public void ReturnRocketToPool(GameObject rocket)
    {
        rocket.SetActive(false);
    }
}

// thanks to chat.openai.com for the script :)