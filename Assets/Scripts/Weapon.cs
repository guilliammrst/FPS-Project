using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public Camera playerCamera;

    //Crosshair variables 
    public GameObject crosshair;
    public readonly float crosshairSize = 40f;
    public Texture2D baseCrosshair;
    public Texture2D sniperCrosshair;

    //Camera Zoom variables
    public float baseFOV = 60f;
    public bool zooming = false;
    public bool zoomed = false;
    public float zoomTime = 0.5f;
    public float zoomMultiplyer = 2f;
    public float zoomFOV = 5f;


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
    public GameObject weaponPrefabWithArm;
    public bool isReloading;
    private bool isToBottom = true;

    // Muzzle effect variables 
    public GameObject muzzleEffect;

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
        crosshair = GameObject.Find("Crosshair");
        baseCrosshair = (Texture2D)Resources.Load("HUD/crosshair");
        sniperCrosshair = (Texture2D)Resources.Load<Texture>("HUD/sniperscope");
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
        if (weapon.CompareTag("PlayerWeapon") && !SceneManager.Instance.gamePaused)
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

                if (Input.GetKeyDown(KeyCode.R) || magazineBullets == 0)
                {
                    isReloading = true;
                                      
                }

                if (isReloading)
                {
                    Reload();
                }

                //HUD
                bulletCountText.text = $"{magazineBullets}/{numberOfBulletsRemaining}";
            }

            // Zooming function
            
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    print(weapon.name);
                    if (!zooming && !isReloading)
                    {
                        zooming = true;
                    }


                }
            

            zoomFunction();
        }
        
    }

    private void FireWeapon()
    {
        muzzleEffect.GetComponent<ParticleSystem>().Play();

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
        bullet.GetComponent<Bullet>().shooter = "Player";

        // Destroy the bullet after a certain amount of time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifeTime));

        magazineBullets--;

        if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Auto)
        {
            if (weapon.name.Contains("Colt"))
            {
                SoundManager.Instance.audioSource.PlayOneShot(SoundManager.Instance.coltSound);
            }
            else if (weapon.name.Contains("Sniper"))
            {
                SoundManager.Instance.audioSource.PlayOneShot(SoundManager.Instance.sniperSound);
            }
            else if (weapon.name.Contains("AK-47"))
            {
                SoundManager.Instance.audioSource.PlayOneShot(SoundManager.Instance.ak47Sound);
            }
        }
        
        // Burst
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            if (weapon.name.Contains("Shotgun"))
            {
                magazineBullets++;

                if (burstBulletsLeft == bulletsPerBurst)
                {
                    SoundManager.Instance.audioSource.PlayOneShot(SoundManager.Instance.shotgunSound);
                }
            }
            else if (weapon.name.Contains("Uzi") && burstBulletsLeft == bulletsPerBurst)
            {
                SoundManager.Instance.audioSource.PlayOneShot(SoundManager.Instance.uziSound);
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

                if (zoomed)
                {
                    zooming = true;
                }

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
            if (isToBottom)
            {
                SoundManager.Instance.audioSource.PlayOneShot(SoundManager.Instance.removeMagSound);

                isToBottom = false;
            }

            weapon.transform.position = new Vector3(weapon.transform.position.x, weapon.transform.position.y - (0.5f / (reloadTime / 3)), weapon.transform.position.z);
        }
        else if (IsReloadingTime > reloadTime - (reloadTime / 3))
        {
            if (!isToBottom)
            {
                SoundManager.Instance.audioSource.PlayOneShot(SoundManager.Instance.reloadingSound);

                isToBottom = true;
            }

            weapon.transform.position = new Vector3(weapon.transform.position.x, weapon.transform.position.y + (0.5f / (reloadTime / 3)), weapon.transform.position.z);
        }
    }


    private void zoomFunction()
    {
        if (zooming)
        {

            if (!zoomed)
            {
                if (weapon.name.Contains("Sniper"))
                {
                    crosshair.GetComponent<RawImage>().texture = sniperCrosshair;
                    crosshair.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);

                    if (Camera.main.fieldOfView > zoomFOV)
                    {
                        Camera.main.fieldOfView -= zoomTime * zoomMultiplyer;
                    }
                    else
                    {
                        zooming = false;
                        zoomed = true;
                    }
                }
                else
                {
                    Camera.main.fieldOfView -= 20;

                    weapon.transform.position = weapon.GetChild(0).transform.position;
                    weapon.GetChild(1).gameObject.SetActive(false);
                    crosshair.SetActive(false);

                    zooming = false;
                    zoomed = true;
                }
            }
            else
            {
                if (weapon.name.Contains("Sniper"))
                {
                    if (Camera.main.fieldOfView < baseFOV)
                    {
                        Camera.main.fieldOfView += zoomTime * zoomMultiplyer;
                    }
                    else
                    {
                        zooming = false;
                        zoomed = false;
                        crosshair.GetComponent<RawImage>().texture = baseCrosshair;
                        crosshair.GetComponent<RectTransform>().sizeDelta = new Vector2(crosshairSize, crosshairSize);
                    }
                }
                else
                {
                    Camera.main.fieldOfView = baseFOV;

                    weapon.transform.position = weapon.parent.transform.position;
                    weapon.GetChild(1).gameObject.SetActive(true);
                    crosshair.SetActive(true);

                    zooming = false;
                    zoomed = false;
                }
            }
        }
    }
}
