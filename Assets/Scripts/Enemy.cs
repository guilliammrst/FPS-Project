using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public float patrolRange = 100f; 
    public float chaseRange = 40f;
    public float fireRange;
    public float moveSpeed = 4f; 

    private Vector3 patrolPoint; 
    private Transform player; 
    private bool isChasing = false;

    private Vector3 moveDirection;

    public Transform weapon;

    public WeaponEnemy WeaponEnemy;

    void Start()
    {
        fireRange = chaseRange / 2;
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("PlayerTag").transform;

        SetRandomPatrolPoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < fireRange)
        {
            WeaponEnemy.FireWeapon();
        }
        else if (distanceToPlayer < chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            isChasing = false;
        }
    
        if (!isChasing)
        {
            Patrol();
        }
    
        if (isChasing)
        {
            transform.LookAt(player);
        }
        else
        {
            transform.LookAt(patrolPoint);
        }
    }


    void FireWeapon()
    {
        WeaponEnemy.FireWeapon();
    }

    void Patrol()
    {
        if (Vector3.Distance(transform.position, patrolPoint) < 1f)
        {
            SetRandomPatrolPoint();
        }

        transform.position = Vector3.MoveTowards(transform.position, patrolPoint, moveSpeed * Time.deltaTime);
    }

    void SetRandomPatrolPoint()
    {
        Vector3 point;
        do
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized * patrolRange;
            point = new Vector3(randomDirection.x, 0, randomDirection.y) + transform.position;
        }
        while (point.x < -100 || point.x > 100 || point.z < -100 || point.z > 100);

        patrolPoint = point;

        Patrol();
    }

    void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        isChasing = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
