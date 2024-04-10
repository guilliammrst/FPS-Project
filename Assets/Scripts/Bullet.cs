using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;

    private void OnCollisionEnter(Collision objectWeHit)
    {
        GameObject hit = objectWeHit.gameObject;
        if (hit.transform.parent != null && hit.transform.parent.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = hit.transform.parent.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                if (objectWeHit.gameObject.CompareTag("EnemyHead"))
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

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }
        else if (hit.CompareTag("Wall"))
        {
            print("hit a wall");

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }
        else if (hit.CompareTag("Ground"))
        {
            print("hit the ground");

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }
    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];

        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
            );

        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}

