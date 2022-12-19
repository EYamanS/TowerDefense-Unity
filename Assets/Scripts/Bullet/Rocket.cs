using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float damage;
    private float impactRadius = .5f;

    
    
    // Update is called once per frame
    void Update()
    {
       
        Vector3 targetDir = target.position - transform.position;
        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, target.position - transform.position));
        transform.position += targetDir.normalized * speed * Time.deltaTime;

        if ((transform.position - target.position).magnitude <= impactRadius)
        {
            BulletPool.Instance.ReturnRocketToPool(this.gameObject);
            target.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }
    }


}
