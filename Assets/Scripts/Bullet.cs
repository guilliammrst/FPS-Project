using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;
        if (hit.transform.parent != null && hit.transform.parent.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = hit.transform.parent.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                if (collision.gameObject.CompareTag("EnemyHead"))
                {
                    damage *= 2;
                }
                enemyHealth.TakeDamage(damage);

                print("Enemy HP: " + enemyHealth.currentHealth);

                Destroy(gameObject);
            }
        }
        else if (hit.CompareTag("Target"))
        {
            print("hit " + hit.name + "!");
            Destroy(gameObject);
        }
        else if (hit.CompareTag("Wall"))
        {
            print("hit a wall");
            Destroy(gameObject);
        }
    }
}

