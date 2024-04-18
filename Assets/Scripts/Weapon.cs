using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public Camera playerCamera;

    // Shooting variables
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    //Burst variables
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    // Spread variables
    public float spreadIntensity;

    // Bullet variables
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVellocity = 30;
    public float bulletLifeTime = 3f;

    //Weapon variables
    public int magazineSize = 30;
    public int magazineBullets;

    //Reload variables
    public float reloadTime = 250f;
    public float IsReloadingTime = 0f;
    public Transform weapon;
    public bool isReloading;

    //HUD variables
    public Text bulletCountText;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    public void Start()
    {
        bulletCountText = GameObject.Find("BulletCountHUD").GetComponent<Text>();
        weapon = GameObject.Find("Weapon").GetComponent<Transform>();
    }

    private void Awake()
    {
        readyToShoot = true;
        isReloading = false;
        burstBulletsLeft = bulletsPerBurst;
        magazineBullets = magazineSize;    
    }

    // Update is called once per frame
    void Update()
    {

        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (isShooting && readyToShoot)
        {
            burstBulletsLeft = bulletsPerBurst;
            if ( magazineBullets > 0 )
            {
                if (isReloading == false)
                {
                    FireWeapon();
                    magazineBullets--;
                }

            } 

        }


        if ( Input.GetKeyDown(KeyCode.R) )
        {
            isReloading = true;
        }

        
        Reload();

        if (magazineBullets == 0)
        {
            //print("Press R to Reload !!");
            isReloading = true;// ----> Auto Reload when out of ammo
        }

        //HUD
        bulletCountText.text = magazineBullets.ToString();

    }

    private void FireWeapon()
    {
        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionWithSpread().normalized;

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        // Pointing the bullet in the right direction
        bullet.transform.forward = shootingDirection;

        // Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVellocity, ForceMode.Impulse);

        // Destroy the bullet after a certain amount of time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifeTime));
        
        // Check if we are done shooting
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Burst
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    private Vector3 CalculateDirectionWithSpread()
    {
        // Shooting from the center of the screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // Hit something
            targetPoint = hit.point;
        }
        else
        {
            // Hit nothing
            targetPoint = ray.GetPoint(100);
        }
        
        Vector3 direction = targetPoint - bulletSpawn.position;

        // Adding spread
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    private void Reload()
    {
        if (isReloading == true)
        {
            if (magazineBullets == magazineSize)
            {
                print("Mag already full !!");
            }
            else
            {
                if (IsReloadingTime == reloadTime)
                {
                    magazineBullets = magazineSize;
                    IsReloadingTime = 0f;
                    isReloading = false;
                }
                else
                {
                    ReloadAnimation();
                    IsReloadingTime += 1f;
                } 

            }
        }
        
    }

    private void ReloadAnimation()
    {
        print((IsReloadingTime <= reloadTime / 3) + " / " + (IsReloadingTime >= (reloadTime / 3) * 2));
        if(IsReloadingTime <= reloadTime/3)
        {
            weapon.position = new Vector3(weapon.position.x, weapon.position.y - (0.5f / (reloadTime/3)), weapon.position.z);

        } else if (IsReloadingTime >= (reloadTime / 3)*2) {

            weapon.position = new Vector3(weapon.position.x, weapon.position.y + (0.5f / (reloadTime / 3)), weapon.position.z);
        }
    }
}
