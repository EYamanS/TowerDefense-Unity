using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{

    public List<Vector2> waypoints;

    [HideInInspector] public float speed;
    public float health = 100f;

    [SerializeField] private int waypointIndex = 0;
    [SerializeField] private Vector2 targetPoint;
    [SerializeField] private Image healthBar;

    public float myHealthWillBe = 100f;
   

    // Start is called before the first frame update
    void Start()
    {
        MakeMove();
    }

    // Update is called once per frame
    private void MakeMove()
    {
        targetPoint = waypoints[waypointIndex];

        float distBetweenWaypoints = (transform.position - new Vector3(targetPoint.x,targetPoint.y,0)).magnitude;

        // get duration for dotween from x = v.t lol 
        transform.DOMove(targetPoint, distBetweenWaypoints / speed).SetEase(Ease.Linear).OnComplete(() => {
            
            // update target on move complete!
            waypointIndex++;

            //than just restart 
            MakeMove();
        });        
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        healthBar.fillAmount = health / 100f;

        if (health <= 0)
        {
            Events.EnemyKilled?.Invoke();
            Destroy(this.gameObject);
        }
    }
}
