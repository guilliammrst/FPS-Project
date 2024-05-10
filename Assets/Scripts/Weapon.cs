using System.Collections;
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
    public int bulletDamage = 10;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVellocity = 30;
    public float bulletLifeTime = 3f;

    //Weapon variables
    public int magazineSize = 30;
    public int magazineBullets;
    public int numberOfBulletsRemaining;

    //Reload variables
    public float reloadTime = 250f;
    public float IsReloadingTime = 0f;
    public Transform weapon;
    public GameObject weaponPrefab;
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
        playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        bulletCountText = GameObject.Find("BulletCountHUD").GetComponent<Text>();
    }

    private void Awake()
    {
        readyToShoot = true;
        isReloading = false;
        burstBulletsLeft = bulletsPerBurst;
        magazineBullets = magazineSize;
        numberOfBulletsRemaining = magazineSize * 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (weapon.CompareTag("PlayerWeapon"))
        { 
            if (weapon.parent.name != null)
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
                    if (magazineBullets > 0)
                    {
                        if (isReloading == false)
                        {
                            FireWeapon();
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    isReloading = true;
                }

                if (isReloading)
                {
                    Reload();
                }

                if (magazineBullets == 0)
                {
                    //print("Press R to Reload !!");
                    isReloading = true;// ----> Auto Reload when out of ammo
                }

                //HUD
                bulletCountText.text = $"{magazineBullets}/{numberOfBulletsRemaining}";
            }
        }
    }

    private void FireWeapon()
    {
        readyToShoot = false;

        if (bulletPrefab.GetComponent<Bullet>().damage != bulletDamage)
        {
            bulletPrefab.GetComponent<Bullet>().damage = bulletDamage;
        }

        Vector3 shootingDirection = CalculateDirectionWithSpread().normalized;

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        // Pointing the bullet in the right direction
        bullet.transform.forward = shootingDirection;

        // Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVellocity, ForceMode.Impulse);

        // Destroy the bullet after a certain amount of time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifeTime));

        magazineBullets--;

        // Burst
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            if (weapon.name.Contains("Shotgun"))
            {
                magazineBullets++;
            }

            burstBulletsLeft--;
            FireWeapon();
        }
        // Check if we are done shooting
        else if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
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
        if (magazineBullets == magazineSize)
        {
            print("Mag already full !!");
            isReloading = false;
        }
        else
        {
            if (numberOfBulletsRemaining == 0)
            {
                print("No more bullets !!");
                isReloading = false;
            }
            else
            {
                print("Reloading...");

                if (IsReloadingTime == reloadTime)
                {
                    int bulletsNeeded = magazineSize - magazineBullets;

                    if (numberOfBulletsRemaining < bulletsNeeded)
                    {
                        magazineBullets += numberOfBulletsRemaining;
                        numberOfBulletsRemaining = 0;
                    }
                    else
                    {
                        magazineBullets = magazineSize;
                        numberOfBulletsRemaining -= bulletsNeeded;
                    }

                    IsReloadingTime = 0f;
                    isReloading = false;

                    weapon.transform.position = weapon.parent.transform.position; // Reset weapon position after reload animation
                }
                else
                {
                    IsReloadingTime += 1f;

                    if (IsReloadingTime < reloadTime)
                    {
                        ReloadAnimation();
                    }
                }
            }
        }
    }

    private void ReloadAnimation()
    {
        print((IsReloadingTime <= reloadTime / 3) + " / " + (IsReloadingTime >= reloadTime - (reloadTime / 3)));
        if (IsReloadingTime < reloadTime / 3)
        {
            weapon.transform.position = new Vector3(weapon.transform.position.x, weapon.transform.position.y - (0.5f / (reloadTime / 3)), weapon.transform.position.z);

        }
        else if (IsReloadingTime > reloadTime - (reloadTime / 3))
        {
            weapon.transform.position = new Vector3(weapon.transform.position.x, weapon.transform.position.y + (0.5f / (reloadTime / 3)), weapon.transform.position.z);
        }
    }
}
