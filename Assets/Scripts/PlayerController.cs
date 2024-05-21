using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    private Rigidbody2D body;
    private PlayerDeath death;
    private Vector2 move = Vector2.zero;
    public Vector2 getMove { get { return move; } }
    public string deathScene = "DeathScreen";
    // Animator vars
    public Animator animator;
    public RuntimeAnimatorController[] controllers;
    public RuntimeAnimatorController[] idleControllers;
    private int currentControllerIndex;
    // Health vars
    public int maxHealth = 5;
    int currentHealth;
    public int health { get { return currentHealth; } }

    public float timeInvincible = .5f;
    private bool isInvincible;
    public float damageCooldown;

    void Start()
    {
        MoveAction.Enable();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        death = GetComponent<PlayerDeath>();
        currentControllerIndex = 6; // Idle state
        animator.runtimeAnimatorController = idleControllers[currentControllerIndex];
        currentHealth = maxHealth;
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();
        if (move != Vector2.zero)
        {
            UpdateAnimation(move);
        } 
        else 
        {
            animator.runtimeAnimatorController = idleControllers[currentControllerIndex];
        }

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if(damageCooldown < 0)
            {
                isInvincible = false;
            }
        }
       
    }

    public bool isMoving()
    {
        return move != Vector2.zero;
    }

    public int getCurrentControllerIndex()
    {
        return currentControllerIndex;
    }

    private void FixedUpdate()
    {
        Vector2 position = (Vector2)transform.position + move * 3.0f * Time.deltaTime;
        body.MovePosition(position);

        if (currentHealth <= 0)
        {
            // This object handles death
            death.Death();
        }
    }

    void UpdateAnimation(Vector2 moveDirection)
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        if (angle < 0)
        {
            angle += 360;
        }


        int i = Mathf.RoundToInt(angle / 45.0f) % controllers.Length;
        
        currentControllerIndex = i;
        animator.runtimeAnimatorController = controllers[currentControllerIndex];
    }

    public void ChangeHealth(int amount)
    {
        if (amount > 0)
        {
            if (isInvincible)
                return;
            isInvincible = true;
            damageCooldown = timeInvincible;
        }

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }
}