using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEnemy : MonoBehaviour
{
    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public int bulletDamage = 10;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 20f;
    public float bulletLifeTime = 2f;
    public float shootingDelay = 3f;
    public int bulletsPerBurst = 3;
    public bool readyToShoot = true;

    public ShootingMode shootingMode = ShootingMode.Single;
    private Transform player;

    public GameObject muzzleEffect;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerTag").transform;
    }

    public void FireWeapon() 
    {
        if (!readyToShoot)
        {
            return;
        }

        if (bulletPrefab.GetComponent<Bullet>().damage != bulletDamage)
        {
            bulletPrefab.GetComponent<Bullet>().damage = bulletDamage;
        }

        muzzleEffect.GetComponent<ParticleSystem>().Play();

        switch (shootingMode)
        {
            case ShootingMode.Single:
                FireSingleShot();
                Invoke("ResetShot", shootingDelay);
                break;
            case ShootingMode.Burst:
                StartCoroutine(FireBurst());
                break;
            case ShootingMode.Auto:
                FireSingleShot();
                Invoke("ResetShot", shootingDelay / 10); 
                break;
        }
    }

    void FireSingleShot()
    {
        if (gameObject.name.Contains("Colt"))
        {
            SoundManager.Instance.audioSource.PlayOneShot(SoundManager.Instance.coltSound);
        }
        else if (gameObject.name.Contains("AK-47"))
        {
            SoundManager.Instance.audioSource.PlayOneShot(SoundManager.Instance.ak47Sound);
        }

        Vector3 shootingDirection = (player.position - transform.position).normalized;
    
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        bullet.GetComponent<Bullet>().shooter = "Enemy";

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifeTime));
    
        readyToShoot = false;
        Invoke("ResetShot", shootingDelay);
    }

    IEnumerator FireBurst()
    {
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            FireSingleShot();
            yield return new WaitForSeconds(shootingDelay / bulletsPerBurst);
        }
        readyToShoot = false;
        Invoke("ResetShot", shootingDelay);
    }

    IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    void ResetShot()
    {
        readyToShoot = true;
    }
}