using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballHit : MonoBehaviour
{
    //public int damage = 1;
    public float extinguishDelay = 3f;
    //private bool hitWall = false;
    //private bool hitEnemy = false;
    public RuntimeAnimatorController extinguish;
    public float animationLength = 0.61f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Extinguish", extinguishDelay);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Extinguish();
        } else if (collision.CompareTag("Wall"))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Extinguish();
        }
    }

    private void Extinguish()
    {
        Animator animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = extinguish;
        Invoke("DestroyObj", animationLength);
    }
    private void DestroyObj() 
    {
        Destroy(gameObject);
    }
}
