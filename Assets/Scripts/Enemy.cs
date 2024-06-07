using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    public WeaponEnemy WeaponEnemy;

    private Animator animator;
    private bool isDied = false;

    public float despawnDelayAfterDeath = 5f;
    
    public GameObject weaponPrefabToDisplay;
    
    void Start()
    {
        fireRange = chaseRange - chaseRange / 3;
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("PlayerTag").transform;

        animator = GetComponent<Animator>();

        SetRandomPatrolPoint();
    }

    void Update()
    {
        if (isDied)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < fireRange)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isFiring", true);
            FireWeapon();
        }
        else
        {
            animator.SetBool("isFiring", false);
            animator.SetBool("isWalking", true);

            if (distanceToPlayer < chaseRange)
            {
                ChasePlayer();
            }
            else
            {
                isChasing = false;
            }
        }
    
        if (!isChasing)
        {
            Patrol();
        }
    
        if (isChasing)
        {
            transform.LookAt(player);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
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
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("DAMAGE"))
        {
            return;
        }

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
        patrolPoint.y = 0;

        Patrol();
    }

    void ChasePlayer()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("DAMAGE"))
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        isChasing = true;
    }

    public void TakeDamage(int damage)
    {
        if (isDied)
        {
            return;
        }

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isFiring", false);
            animator.SetTrigger("DIE");
            isDied = true;

            StartCoroutine(Die(despawnDelayAfterDeath));
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isFiring", false);
            animator.SetTrigger("DAMAGE");
        }
    }

    private IEnumerator Die(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject instantiateWeapon = Instantiate(weaponPrefabToDisplay, transform.position + new Vector3(0, 1, 1), transform.rotation * new Quaternion(1, 0, 1, 1));
        if (instantiateWeapon.GetComponent<Rigidbody>() == null)
        {
            instantiateWeapon.AddComponent<Rigidbody>();
            instantiateWeapon.GetComponent<Rigidbody>().mass = 50f;
        }

        Destroy(gameObject);
    }
}
