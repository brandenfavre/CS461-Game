using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstNPCLeaves : MonoBehaviour
{
    [SerializeField] private GameObject npc;
    [SerializeField] private Canvas prompt;
    [SerializeField] private RuntimeAnimatorController idle;
    [SerializeField] private RuntimeAnimatorController walkBack;
    private Animator animator;
    private Rigidbody2D body;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        animator.runtimeAnimatorController = idle;
        prompt.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        prompt.enabled = true;
        Invoke("Leave", 2f);
    }

    void Leave()
    {
        animator.runtimeAnimatorController = walkBack;
        body.velocity = Vector3.up * 1f;
        Invoke("Destroy", .6f);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
