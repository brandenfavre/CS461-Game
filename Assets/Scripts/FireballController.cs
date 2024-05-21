using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public GameObject fireballPrefab;
    public float fireballSpeed = 5f;
    public float fireballCooldown = 2f;
    private PlayerController player;
    private bool canShoot = true;
    [SerializeField] private 
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canShoot)
        {
            canShoot = false;
            ShootFireball();
            UIHandler.instance.fireballTimer(fireballCooldown);
            Invoke(nameof(resetCooldown), fireballCooldown);
        }
    }

    void ShootFireball()
    {

        Vector2 fireballDirection = Vector2.down;
        float angle = 0f;

        // Calculate angle and direction based on players controller index (where the player is facing)
        switch (player.getCurrentControllerIndex())
        {
            case 0:
                fireballDirection = new Vector2(1, 0); // East 
                angle = 90f;
                break;
            case 1:
                fireballDirection = new Vector2(1, 1); // NE
                angle = 135f;
                break;
            case 2:
                fireballDirection = new Vector2(0, 1); // North
                angle = 180f;
                break;
            case 3:
                fireballDirection = new Vector2(-1, 1); // NW
                angle = -135f;
                break;
            case 4:
                fireballDirection = new Vector2(-1, 0); // West
                angle = -90f;
                break;
            case 5:
                fireballDirection = new Vector2(-1, -1); // SW
                angle = -45f;
                break;
            case 6:
                fireballDirection = new Vector2(0, -1); // South
                break;
            case 7:
                fireballDirection = new Vector2(1, -1); // SE
                angle = 45f;
                break;
            case 8:
                fireballDirection = new Vector2(0, -1); // South
                break;
        }

        // Make fireball object
        Vector2 fireballPoint = (Vector2)(transform.position);
        GameObject fireball = Instantiate(fireballPrefab, fireballPoint, Quaternion.identity);

        // Change velocity
        fireball.GetComponent<Rigidbody2D>().velocity = fireballDirection.normalized * fireballSpeed;

        // Rotate fireball for correct direciton
        fireball.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Make it so fireball is always behind player
        fireball.transform.position += new Vector3(0, 0, 1f);
    }
    void resetCooldown()
    {
        canShoot = true;
    }
}
