using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public string shooter;

    public GlobalReferences globalReferences;

    void Start()
    {
        globalReferences = GameObject.Find("GlobalReferences").GetComponent<GlobalReferences>();
    }

    private void OnCollisionEnter(Collision objectWeHit)
    {
        GameObject hit = objectWeHit.gameObject;

        if (hit.name.Contains(shooter) && shooter != null && shooter != "")
        {
            return;
        }

        if (hit.CompareTag("PlayerTag"))
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
        else if (hit.CompareTag("Enemy"))
        {
            Enemy enemy = hit.GetComponent<Enemy>();

            if (enemy != null)
            {
                if (objectWeHit.gameObject.CompareTag("EnemyHead"))
                {
                    damage *= 2;
                }
                enemy.TakeDamage(damage);

                if ( enemy.currentHealth <= 0 )
                {
                    globalReferences.ActiveHitMarkerRed();
                }
                else {
                    globalReferences.ActiveHitMarkerWhite();
                }

                print("Enemy HP: " + enemy.currentHealth);
            }
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