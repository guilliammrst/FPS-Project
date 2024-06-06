using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public GameObject shooter;

    private void OnCollisionEnter(Collision objectWeHit)
    {
        GameObject hit = objectWeHit.gameObject;

        if (hit == shooter)
        {
            return;
        }

        if (hit.transform.parent != null && hit.transform.parent.CompareTag("Enemy"))
        {
            Enemy Enemy = hit.transform.parent.GetComponent<Enemy>();
    
            if (Enemy != null)
            {
                if (objectWeHit.gameObject.CompareTag("EnemyHead"))
                {
                    damage *= 2;
                }
                Enemy.TakeDamage(damage);
    
                print("Enemy HP: " + Enemy.currentHealth);
            }
        }
        else if (hit.CompareTag("PlayerTag"))
        {
            PlayerSystem player = hit.GetComponent<PlayerSystem>();
            if (player != null)
            {
                if (objectWeHit.gameObject.CompareTag("PlayerHead"))
                {
                    damage *= 2;
                }
                player.TakeDamage(damage);
    
                print("Player HP: " + player.currentHealth);
            }
        }
        else if (hit.CompareTag("Target") || hit.CompareTag("Wall") || hit.CompareTag("Ground"))
        {
            print("Hit " + hit.name + "!");
    
            CreateBulletImpactEffect(objectWeHit);
        }
    
        Destroy(gameObject);
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