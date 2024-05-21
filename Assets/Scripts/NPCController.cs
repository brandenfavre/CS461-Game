using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    private Transform playerTransform;
    public float speed = 3.0f;
    private Animator animator;
    public RuntimeAnimatorController[] controllers;
    [SerializeField] private RuntimeAnimatorController[] idleControllers;
    private int currentControllerIndex;
    private Rigidbody2D body;
    private PlayerController player;
    private SpriteRenderer sprite;
    public float minDistanceToPlayer = 1.5f;
    private bool withPlayer = false;
    private bool findPlayer = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        currentControllerIndex = 8; // Idle
        animator.runtimeAnimatorController = controllers[currentControllerIndex];
        player = FindObjectOfType<PlayerController>();
        playerTransform = player.transform;
        sprite = GetComponent<SpriteRenderer>();

        // NPC only has access to this script when dialouge is done, so no need to check if NPC is with player
        withPlayer = true;
    }

    // Update is called once per frame
    void Update()
    {
        // If I don't have player, if the player is moving, and i'm with player I move, or if I need to find the player
        if (playerTransform != null && player.getMove != Vector2.zero && withPlayer || findPlayer)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > minDistanceToPlayer)
            {
                findPlayer = true;
                if (transform.position.y < playerTransform.position.y)
                {   // Player in front of NPC
                    sprite.sortingOrder = playerTransform.GetComponent<SpriteRenderer>().sortingOrder + 3;
                }
                else
                {   // Player behind NPC
                    sprite.sortingOrder = playerTransform.GetComponent<SpriteRenderer>().sortingOrder - 3;
                }
                body.velocity = direction * speed;
                UpdateAnimation(direction);
            } 
            else
            {
                findPlayer = false;
                body.velocity = Vector2.zero;
            }
        }
        else
        {
            body.velocity = Vector2.zero;
            animator.runtimeAnimatorController = idleControllers[currentControllerIndex];
        }
    }

    public bool isWithPlayer()
    {
        return withPlayer;
    }
    void UpdateAnimation(Vector2 moveDirection)
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        if (angle < 0)
        {
            angle += 360;
        }

        int i = Mathf.RoundToInt(angle / 45.0f) % controllers.Length;
        if (i == 8) // Moving from southeast to east, sets to idle state when it should be east
        {
            i = 0; // east animation
        }

        currentControllerIndex = i;
        animator.runtimeAnimatorController = controllers[currentControllerIndex];
    }
}
