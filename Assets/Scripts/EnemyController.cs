using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float detectionRange = 5f;
    public int attackDamage = 1;
    public float chargeSpeed = 1.5f;
    public LayerMask obstacleLayer;

    public RuntimeAnimatorController[] moveController;
    public RuntimeAnimatorController[] attackController;

    [SerializeField] private int enemyHealth = 1;

    private Animator animator;
    private Rigidbody2D body;
    private Transform player;

    private PlayerController playerController;

    private Vector2 moveDirection;
    public float minDistanceToPlayer = .5f;

    private bool attacking = false; // Flag for currently attacking
    private bool attackFinished = true; // Flag for checking if animation is finished
    public float attackCooldown = 0.5f;
    public bool damageCooldown = false;
    private bool canAttack = true; // Flag for able to attack

    private int currentControllerIndex = 4;

    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator.runtimeAnimatorController = moveController[currentControllerIndex];
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (!canAttack) // Make enemy stand still when in cooldown
        {
            moveDirection = Vector2.zero;
            animator.runtimeAnimatorController = moveController[4]; // Idle
        }
        // Messy, but works kinda
        else if (Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > minDistanceToPlayer) //&& canAttack)
            {
                if (attackFinished) // Only move if attack animation finished
                {
                    attacking = false;
                    moveDirection = (player.position - transform.position).normalized;

                    // Kind of handles if there's a wall inbetween, should probably do a different way
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, 1f, obstacleLayer);
                    if (hit.collider != null)
                    {
                        Vector2 avoidDirection = Vector2.Perpendicular(hit.normal).normalized;
                        moveDirection = avoidDirection;
                    }
                    UpdateAnimation(); // Only updates movement
                }
            }
            else if (!attacking && attackFinished && canAttack) // Only attack if not attacking and attack animation finished
                Attack();
        }
        else // Player out of range
        {
            moveDirection = Vector2.zero;
            animator.runtimeAnimatorController = moveController[4]; // Idle
        }
       /* if (!canAttack) // Make enemy stand still when in cooldown
        {
            moveDirection = Vector2.zero;
            animator.runtimeAnimatorController = moveController[4]; // Idle
        }*/
    }
    public int getIndex()
    {
        return currentControllerIndex;
    }
    public bool isAttacking()
    {
        return attacking;
    }
    void FixedUpdate()
    {
        if (!attacking)
        {
            body.velocity = moveDirection * moveSpeed;
        } 
        else
        {
            body.velocity = moveDirection * moveSpeed * chargeSpeed; // Charge attack
        }
    }

    void Attack()
    {
        attacking = true;
        attackFinished = false;
        animator.runtimeAnimatorController = attackController[currentControllerIndex];
        // Calculate attack duration from animation length
        float attackDuration = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        Invoke("FinishAttack", attackDuration); // Set a delay to finish the attack
    }

    void FinishAttack()
    {
        attackFinished = true;
        attacking = false;
   
        canAttack = false;
        Invoke("resetCooldown", attackCooldown);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collision is with the player and the enemy is in midAttack
        if (other.CompareTag("Player") && attacking)
        {
            playerController.ChangeHealth(attackDamage);
        }
        if (other.CompareTag("Fireball"))
        {
            other.gameObject.tag = "Untagged"; // Takes away "fireball" tag, so the fireball can't work twice
            enemyHealth -= 1; // Deal damage
            checkHealth(); // Check enemies health
            damageCooldown = true;
        }
    }

    void checkHealth()
    {
        if(enemyHealth <= 0)
            Destroy(gameObject); // Kill enemy
    }

    void resetCooldown()
    {
        canAttack = true;
    }

    void UpdateAnimation()
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        int i = Mathf.RoundToInt((angle + 180) / 90f) % 4;
        currentControllerIndex = i; // Essentially direction, used with attack as well in Attack()
        animator.runtimeAnimatorController = moveController[currentControllerIndex];
    }
}
