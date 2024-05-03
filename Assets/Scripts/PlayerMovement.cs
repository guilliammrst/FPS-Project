using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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

    
    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<CharacterController>();
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

        
        if (hasTakenDamage)
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= 5f)
            {
                Heal(healRate);
                damageTimer = 0f;
            }
        }
    }

        public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Réinitialisez le compteur et arrêtez la guérison chaque fois que le joueur prend des dégâts
        hasTakenDamage = true;
        damageTimer = 0f;

        if (currentHealth <= 0)
        {
            Die();
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
    
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}