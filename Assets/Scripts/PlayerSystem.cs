using UnityEngine;
using UnityEngine.UI;

public class PlayerSystem : MonoBehaviour
{
    private CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public int maxHealth = 100;
    public int currentHealth;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float damageTimer = 0f;
    public float healRate = 10f;

    Vector3 velocity;
    
    bool isGrounded;
    bool isMoving;
    bool hasTakenDamage = false;

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);

    public int numberOfGrenades = 0;
    public float throwForce = 40f;
    public float forceMultiplier = 0;
    public GameObject grenadePrefab;
    public GameObject throwableSpawn;
    public Text grenadeCountText;
    
    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<CharacterController>();
        grenadeCountText = GameObject.Find("GrenadeCountHUD").GetComponent<Text>();
    }


    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        
        Vector3 move = transform.right * x + transform.forward * z;

        
        controller.Move(move * speed * Time.deltaTime);

        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        
        controller.Move(velocity * Time.deltaTime);

        if (lastPosition != gameObject.transform.position && isGrounded)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        lastPosition = gameObject.transform.position;

        if (Input.GetKey(KeyCode.F))
        {
            forceMultiplier += Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            if (numberOfGrenades > 0)
            {
                ThrowGrenade();

                forceMultiplier = 0;
            }
        }

        if (hasTakenDamage)
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= 5f)
            {
                Heal(healRate);
                damageTimer = 0f;
            }
        }

        grenadeCountText.text = numberOfGrenades.ToString();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Réinitialisez le compteur et arrêtez la guérison chaque fois que le joueur prend des dégâts
        hasTakenDamage = true;
        damageTimer = 0f;

        if (currentHealth <= 0)
        {
            SoundManager.Instance.audioSource.PlayOneShot(SoundManager.Instance.manDie);

            Die();
        }
        else
        {
            SoundManager.Instance.audioSource.PlayOneShot(SoundManager.Instance.manHurt);
        }
    }

    void Heal(float amount)
    {
        currentHealth += (int)amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
            hasTakenDamage = false;
        }
    }

    void Die()
    {
        Debug.Log("Le joueur est mort.");
    
        SceneManager.Instance.GameOver();
    }

    private void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        grenade.GetComponent<Throwable>().hasBeenThrown = true;
        numberOfGrenades--;
    }
}