using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class PlayerDeath : MonoBehaviour
{
    private PlayerController player;
    private Transform playerTransform;
    private FireballController fireball;
    private Animator animator;
    private SpriteRenderer sprite;
    [SerializeField] private string deathScene;
    [SerializeField] private RuntimeAnimatorController deathController;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();
        fireball = GetComponent<FireballController>();
        animator = GetComponent<Animator>();
        playerTransform = player.transform;
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Death()
    {
        player.enabled = false;
        fireball.enabled = false;
        // Want to ensure that the death animation is played above everything else, to emphasize it
        sprite.sortingOrder = playerTransform.GetComponent<SpriteRenderer>().sortingOrder + 10;
        animator.runtimeAnimatorController = deathController;
        float deathDuration = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length - 0.1f;
        Invoke("Destroy", deathDuration);
    }
    private void Destroy()
    {
        SceneManager.LoadScene(deathScene);
    }
}
